---

## 2. `backend/README.md`

```markdown
# AlgoForge Backend

Backend API of the AlgoForge algorithm learning and competitive programming platform.

Built with **ASP.NET Core 8**, **Entity Framework Core** and **PostgreSQL**.

## 🏗️ Architecture

The backend follows a layered architecture based on Clean Architecture principles.

```text
AlgoForge.API
      │
      ▼
AlgoForge.Application
      │
      ▼
AlgoForge.Domain
      ▲
      │
AlgoForge.Infrastructure
      │
      ▼
PostgreSQL
📂 Structure
backend/
│
├── src/
│   ├── AlgoForge.API/
│   ├── AlgoForge.Application/
│   ├── AlgoForge.Domain/
│   └── AlgoForge.Infrastructure/
│
├── tests/
│   ├── AlgoForge.UnitTests/
│   └── AlgoForge.IntegrationTests/
│
├── Dockerfile
└── README.md
🧩 Main Modules

The backend currently contains functionality related to:

Authentication
Questions
Categories
Submissions
Contests
Leaderboard
Profile
AI
🛠️ Technologies
C#
ASP.NET Core 8
Entity Framework Core
PostgreSQL
MediatR
JWT Authentication
Docker
🚀 Run Locally
cd backend
dotnet restore
dotnet build
dotnet run --project src/AlgoForge.API
🗄️ Database

AlgoForge uses PostgreSQL as its database.

Database configuration is provided through the application's configuration/environment settings.

Do not commit database credentials or other secrets to the repository.

🐳 Docker

The backend can be built and run using Docker.

docker build -t algoforge-api .
🌐 Production

The backend is deployed on Render using Docker.

Render
   │
   ▼
ASP.NET Core API
   │
   ▼
PostgreSQL
🧪 Tests

Tests are located under:

tests/
├── AlgoForge.UnitTests/
└── AlgoForge.IntegrationTests/

Run:

dotnet test
