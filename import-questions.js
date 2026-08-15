// AlgoForge - questions.json içindeki soruları canlı (Render) API'ye toplu olarak ekler.
//
// Kullanım:
//   node import-questions.js
//
// Node 18+ gerekli (yerleşik fetch kullanıyor). Node sürümünü kontrol etmek için: node -v

const API_BASE = process.env.API_BASE_URL || "https://algoforge-api-b3b9.onrender.com";
const ADMIN_EMAIL = process.env.ADMIN_EMAIL;
const ADMIN_PASSWORD = process.env.ADMIN_PASSWORD;

if (!ADMIN_EMAIL || !ADMIN_PASSWORD) {
  console.error("ADMIN_EMAIL ve ADMIN_PASSWORD environment variable olarak tanımlanmalı.");
  process.exit(1);
}

const fs = require("fs");
const path = require("path");

// Render'in ucretsiz plani bir sure kullanilmayinca "uyuyor" ve ilk istekte
// ayilmasi 30-60 saniye surebiliyor. Node'un varsayilan fetch connect timeout'u
// (10sn) buna yetmiyor, bu yuzden AbortSignal ile daha uzun bir sure taniyoruz
// ve basarisiz olursa birkac kez tekrar deniyoruz.
async function fetchWithRetry(url, options = {}, { retries = 5, timeoutMs = 60000 } = {}) {
  for (let attempt = 1; attempt <= retries; attempt++) {
    try {
      const controller = new AbortController();
      const timer = setTimeout(() => controller.abort(), timeoutMs);
      const res = await fetch(url, { ...options, signal: controller.signal });
      clearTimeout(timer);
      return res;
    } catch (err) {
      if (attempt === retries) throw err;
      console.log(`   (Baglanti denemesi ${attempt}/${retries} basarisiz, tekrar deneniyor... - Render uyaniyor olabilir)`);
      await new Promise((r) => setTimeout(r, 5000));
    }
  }
}

async function main() {
  console.log("0) Render servisi uyandiriliyor (ilk istek 30-60sn surebilir, sabirla bekle)...");
  try {
    await fetchWithRetry(`${API_BASE}/api/questions`, {}, { retries: 3, timeoutMs: 60000 });
    console.log("   Servis ayakta.");
  } catch (err) {
    console.error("   Servis uyandirilamadi:", err.message ?? err);
    process.exit(1);
  }

  console.log("1) Admin olarak giriş yapılıyor...");
  const loginRes = await fetchWithRetry(`${API_BASE}/api/auth/login`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ email: ADMIN_EMAIL, password: ADMIN_PASSWORD }),
  });

  if (!loginRes.ok) {
    const text = await loginRes.text();
    console.error(`Giriş başarısız (HTTP ${loginRes.status}):`, text);
    process.exit(1);
  }

  const loginData = await loginRes.json();
  const token = loginData.accessToken;
  if (!token) {
    console.error("Yanıtta accessToken bulunamadı:", loginData);
    process.exit(1);
  }
  console.log("   Giriş başarılı, token alındı.");

  console.log("2) Render'da hâlihazırda var olan sorular kontrol ediliyor...");
  const existingTitles = new Set();
  let page = 1;
  const pageSize = 100;
  while (true) {
    const res = await fetchWithRetry(`${API_BASE}/api/questions?page=${page}&pageSize=${pageSize}`);
    if (!res.ok) {
      console.error(`Mevcut sorular alınamadı (HTTP ${res.status}). Devam ediliyor, çakışma kontrolü atlanacak.`);
      break;
    }
    const data = await res.json();
    for (const q of data.items ?? data.Items ?? []) {
      existingTitles.add(q.title ?? q.Title);
    }
    const total = data.totalCount ?? data.TotalCount ?? 0;
    if (page * pageSize >= total || (data.items ?? data.Items ?? []).length === 0) break;
    page++;
  }
  console.log(`   Render'da şu an ${existingTitles.size} soru var.`);

  const jsonPath = path.join(__dirname, "questions.json");
  const questions = JSON.parse(fs.readFileSync(jsonPath, "utf-8"));
  console.log(`3) questions.json içinde ${questions.length} soru bulundu. Ekleme başlıyor...\n`);

  let added = 0;
  let skipped = 0;
  let failed = 0;

  for (const q of questions) {
    if (existingTitles.has(q.title)) {
      console.log(`   [ATLANDI] "${q.title}" zaten mevcut.`);
      skipped++;
      continue;
    }

    const payload = {
      title: q.title,
      difficulty: q.difficulty,
      description: q.description,
      categoryId: q.categoryId,
      timeLimitMs: q.timeLimitMs,
      memoryLimitMb: q.memoryLimitMb,
      testCases: q.testCases.map((tc) => ({
        input: tc.input,
        expectedOutput: tc.expectedOutput,
        isHidden: tc.isHidden,
      })),
    };

    const res = await fetchWithRetry(`${API_BASE}/api/questions`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${token}`,
      },
      body: JSON.stringify(payload),
    });

    if (res.ok) {
      console.log(`   [EKLENDİ] "${q.title}"`);
      added++;
    } else {
      const text = await res.text();
      console.error(`   [HATA] "${q.title}" eklenemedi (HTTP ${res.status}): ${text}`);
      failed++;
    }
  }

  console.log("\n--- Özet ---");
  console.log(`Eklenen : ${added}`);
  console.log(`Atlanan : ${skipped} (zaten mevcuttu)`);
  console.log(`Hatalı  : ${failed}`);
}

main().catch((err) => {
  console.error("Beklenmeyen hata:", err);
  process.exit(1);
});
