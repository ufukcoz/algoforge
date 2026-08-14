# AlgoForge

AlgoForge is an algorithm learning and competitive programming platform.

The project provides a desktop application where users can practice programming problems, write code, submit solutions and use AI-assisted learning features.

## 🚀 Features

- Algorithm and programming questions
- Code editor with Monaco Editor
- Code submissions
- Code execution through Judge0
- User authentication
- Contests
- Leaderboard
- AI assistant
- Desktop application

## 🏗️ Architecture

```text
                    AlgoForge Desktop
                  Electron + React
                          │
                          │ REST API
                          ▼
                 ASP.NET Core Web API
                          │
              ┌───────────┼───────────┐
              ▼           ▼           ▼
         PostgreSQL     Judge0      AI API

🛠️ Technologies
Backend
C#
ASP.NET Core 8
Entity Framework Core
PostgreSQL
JWT Authentication
MediatR
Desktop
Electron
React
TypeScript
Vite
Monaco Editor
Electron Builder
Infrastructure
Docker
GitHub Actions
Render
PostgreSQL

📂 Project Structure
AlgoForge/
│
├── backend/
│   ├── src/
│   │   ├── AlgoForge.API/
│   │   ├── AlgoForge.Application/
│   │   ├── AlgoForge.Domain/
│   │   └── AlgoForge.Infrastructure/
│   │
│   └── tests/
│
├── desktop/
│   ├── src/
│   └── electron/
│
├── .github/
│   └── workflows/
│
├── questions.json
├── render.yaml
└── README.md

🌐 Deployment

The backend is deployed on Render using Docker.

The production environment uses PostgreSQL for persistent data.

GitHub
   │
   ▼
Render
   │
   ▼
Dockerized ASP.NET Core API
   │
   ▼
PostgreSQL

💻 Local Development
Backend
cd backend
dotnet restore
dotnet build
dotnet run --project src/AlgoForge.API
Desktop
cd desktop
npm install
npm run electron:dev

🧪 Tests

Backend test projects are located under:

backend/tests/

Run tests with:

dotnet test

🗺️ Roadmap
Improve authentication and session management
Expand automated testing
Improve AI assistant
Improve contest features
Add user progress features
Improve desktop application

📌 Status

AlgoForge is currently under active development.

👨‍💻 Author

Ufuk Çöz

GitHub: https://github.com/ufukcoz/algoforge
