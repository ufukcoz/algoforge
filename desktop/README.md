# AlgoForge Desktop — Electron + React + TypeScript

Login/Register ekranı, AlgoForge backend'ine (`http://localhost:5000`) bağlanan çalışır bir masaüstü istemcisi.

## Yapı

```
electron/
  main.ts       → Electron ana süreç (pencereyi oluşturur)
  preload.ts     → renderer ile main süreç arasında güvenli köprü
src/
  api/client.ts  → backend'e fetch istekleri (register, login)
  context/       → AuthContext, oturum durumunu (token) tutar
  pages/
    AuthPage.tsx      → login/register formu
    DashboardPage.tsx → giriş sonrası basit doğrulama ekranı
```

## Kurulum

Bu klasörde (dosyaları indirdiğin yerde):

```bash
npm install
```

## Geliştirme modunda çalıştırma

**Önce backend'in ayakta olduğundan emin ol** (ayrı bir terminalde):
```bash
cd AlgoForge  # backend klasörün
dotnet run --project src/AlgoForge.API
```

**Sonra bu klasörde iki terminal aç:**

Terminal 1 — Vite dev server:
```bash
npm run dev
```

Terminal 2 — Electron'u başlat (Vite server ayağa kalktıktan sonra):
```bash
npx electron .
```

Bir masaüstü penceresi açılacak, terminal-temalı bir login ekranı göreceksin. Kayıt ol veya daha önce backend'i test ederken oluşturduğun `test@algoforge.com` / `Test1234!` ile giriş yap.

## appsettings.json ile port uyuşmazlığı

E�er backend'in farklı bir portta çalışıyorsa (5000 değilse), `src/api/client.ts` dosyasındaki:
```ts
const API_BASE_URL = 'http://localhost:5000/api';
```
satırını kendi portuna göre güncelle.

## CORS hatası alırsan

Backend'deki `Program.cs` içinde CORS policy'si `http://localhost:5173` origin'ine izin veriyor (Vite'ın varsayılan portu). Eğer Vite farklı bir portta açılırsa (terminalde "Local: http://localhost:XXXX" yazan port), backend'deki CORS origin'ini o porta göre güncellemen gerekir.

## Production build (opsiyonel, şimdilik gerekli değil)

```bash
npm run electron:build
```
Bu, `release/` klasörüne kurulabilir bir `.exe` üretir (Windows'ta NSIS installer). Şu an geliştirme aşamasında olduğumuz için buna gerek yok, `npm run dev` + `npx electron .` yeterli.

## Sırada ne var

- Token'ı güvenli şekilde saklamak (şu an sadece React state'te, uygulama kapanınca kayboluyor)
- Question modülü ekranları
- Register'da email doğrulama akışı
