# ---- Build asamasi ----
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Once sadece .csproj dosyalarini kopyala - boylece kod degismedigi surece
# 'dotnet restore' katmani Docker cache'inden tekrar kullanilir, build hizlanir.
COPY src/AlgoForge.Domain/*.csproj src/AlgoForge.Domain/
COPY src/AlgoForge.Application/*.csproj src/AlgoForge.Application/
COPY src/AlgoForge.Infrastructure/*.csproj src/AlgoForge.Infrastructure/
COPY src/AlgoForge.API/*.csproj src/AlgoForge.API/
RUN dotnet restore src/AlgoForge.API/AlgoForge.API.csproj

# Simdi kalan tum kaynak kodunu kopyala ve derle.
COPY src/ src/
RUN dotnet publish src/AlgoForge.API/AlgoForge.API.csproj -c Release -o /app/publish --no-restore

# ---- Calisma (runtime) asamasi ----
# SDK yerine kucuk runtime image kullaniyoruz - final image boyutu ~200MB yerine ~1GB olmaz.
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Render/Docker container'lar genelde PORT ortam degiskeniyle hangi portu dinleyecegini soyler.
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "AlgoForge.API.dll"]
