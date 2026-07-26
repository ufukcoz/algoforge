# AlgoForge — Backend Başlangıç İskeleti

Bu, Sprint 1 için Clean Architecture'a göre kurulmuş .NET solution'ı: Authentication (Register/Login/JWT) çalışır durumda, PostgreSQL'e bağlanmaya hazır.

## Katman yapısı

```
AlgoForge.Domain          → Entities (User), hiçbir şeye bağımlı değil
AlgoForge.Application     → CQRS commands (Register, Login), sadece Domain'e bağımlı
AlgoForge.Infrastructure  → EF Core + PostgreSQL, JWT, BCrypt — Application'a bağımlı
AlgoForge.API             → Controllers, Program.cs — hepsine bağımlı
```

## Gereksinimler

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- PostgreSQL (local kurulum veya Docker: `docker run --name algoforge-db -e POSTGRES_PASSWORD=postgres -e POSTGRES_DB=algoforge -p 5432:5432 -d postgres`)

## Kurulum adımları

1. **appsettings.json'ı düzenle** (`src/AlgoForge.API/appsettings.json`):
   - `ConnectionStrings:DefaultConnection` — kendi PostgreSQL bilgilerinle güncelle
   - `Jwt:Secret` — en az 32 karakterlik rastgele, güçlü bir gizli anahtarla değiştir (bunu asla repoya commit'leme, production'da User Secrets veya environment variable kullan)

2. **Paketleri geri yükle:**
   ```bash
   dotnet restore
   ```

3. **EF Core migration aracını kur (bir kere):**
   ```bash
   dotnet tool install --global dotnet-ef
   ```

4. **İlk migration'ı oluştur ve veritabanını güncelle:**
   ```bash
   dotnet ef migrations add InitialCreate --project src/AlgoForge.Infrastructure --startup-project src/AlgoForge.API
   dotnet ef database update --project src/AlgoForge.Infrastructure --startup-project src/AlgoForge.API
   ```

5. **API'yi çalıştır:**
   ```bash
   dotnet run --project src/AlgoForge.API
   ```
   Swagger arayüzü: `https://localhost:{port}/swagger`

## Test etmek için

```bash
curl -X POST https://localhost:{port}/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{"username":"testuser","email":"test@algoforge.com","password":"Test1234!"}'

curl -X POST https://localhost:{port}/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"test@algoforge.com","password":"Test1234!"}'
```

Login başarılı olursa bir `accessToken` ve `refreshToken` dönmeli.

## Sırada ne var

- Refresh token endpoint'i ve rotasyon mantığı
- Email doğrulama akışı
- Electron + React istemcisinin bu API'ye bağlanması
- Question modülü (Sprint 2)

## Not

Bu ortamda .NET SDK kurulu olmadığı için proje build/test edilemedi. Kendi makinende `dotnet build` ile derleyip hataları kontrol etmeni öneririm — CQRS/DI bağlantılarında küçük bir typo olabilir, ama katman mimarisi ve akış doğru kurgulanmıştır.
