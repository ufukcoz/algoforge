# AlgoForge Desktop - Electron + React + TypeScript

Login/Register ekrani, AlgoForge backend'ine (http://localhost:5000) baglanan calisir bir masaustu istemcisi.

## Yapi

electron/main.ts -> Electron ana surec (pencereyi olusturur)
electron/preload.ts -> renderer ile main surec arasinda guvenli kopru
src/api/client.ts -> backend'e fetch istekleri (register, login)
src/context/ -> AuthContext, oturum durumunu (token) tutar
src/pages/AuthPage.tsx -> login/register formu
src/pages/DashboardPage.tsx -> giris sonrasi basit dogrulama ekrani

## Kurulum

npm install

## Gelistirme modunda calistirma

Once backend calistir (ayri terminalde):
cd backend
dotnet run --project src/AlgoForge.API

Sonra bu klasorde iki terminal ac:

Terminal 1: npm run dev
Terminal 2 (Vite ayaga kalktiktan sonra): npx electron .

Kayit ol veya test@algoforge.com / Test1234! ile giris yap.

## Port uyusmazligi

Backend farkli portta calisiyorsa, src/api/client.ts icindeki API_BASE_URL satirini guncelle.

## CORS hatasi alirsan

Program.cs icindeki CORS policy http://localhost:5173 origin'ine izin veriyor. Vite farkli portta acilirsa backend'deki CORS origin'ini guncelle.

## Production build

npm run electron:build

## Sirada ne var

- Token'i guvenli saklamak
- Question modulu ekranlari
- Email dogrulama akisi
