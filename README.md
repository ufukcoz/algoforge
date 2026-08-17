# AlgoForge

AlgoForge is an algorithm learning and competitive programming platform with a desktop application.

Users can browse algorithm problems, write code with Monaco Editor, submit solutions, participate in contests, view leaderboards and use AI-assisted learning features.

## Features

* Algorithm and data-structure questions
* Problem browsing and filtering
* Monaco Editor
* Code submissions
* Code execution through Judge0
* User authentication
* Role-based authorization
* Contests
* Public and private contest access control
* Contest leaderboard
* Global leaderboard
* User profile
* AI assistant
* Desktop application
* PostgreSQL persistence

## Architecture

```text
                 AlgoForge Desktop
              Electron + React
                       │
                       │ REST API
                       ▼
              ASP.NET Core 8 API
                       │
          ┌────────────┼────────────┐
          ▼            ▼            ▼
     PostgreSQL      Judge0      Gemini API
```

The backend is organized into separate layers:

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
```

## Technologies

### Backend

* C#
* ASP.NET Core 8
* Entity Framework Core
* PostgreSQL
* MediatR
* JWT Authentication
* BCrypt

### Desktop

* Electron
* React
* TypeScript
* Vite
* Monaco Editor
* Electron Builder

### External Services

* Judge0
* Google Gemini
* Resend

### Infrastructure

* Docker
* Render
* GitHub Actions
* GitHub Container Registry

## Security

The backend includes:

* JWT authentication
* Role-based authorization
* BCrypt password hashing
* Refresh-token hashing with SHA-256
* Refresh-token rotation
* Refresh-token family tracking
* Refresh-token reuse detection
* Private contest authorization
* User-scoped submission access
* API rate limiting
* Security response headers
* Centralized exception handling
* Production secrets supplied through environment configuration

## Testing

The repository contains separate unit and integration test projects:

```text
backend/tests/
├── AlgoForge.UnitTests/
└── AlgoForge.IntegrationTests/
```

The tests cover authentication, refresh-token behavior, contest authorization, leaderboard access, role authorization, submission ownership, rate limiting, security headers, CORS behavior and health checks.

Run the backend tests with:

```bash
dotnet test
```

## Deployment

The backend is deployed with Docker on Render and uses PostgreSQL for persistent storage.

```text
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
```

The API exposes:

```text
GET /health
```

for service health checking.

## Local Development

### Backend

```bash
cd backend
dotnet restore
dotnet build
dotnet run --project src/AlgoForge.API
```

### Desktop

```bash
cd desktop
npm install
npm run electron:dev
```

### Docker

```bash
cd backend
docker compose up -d
```

## Project Structure

```text
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
│       ├── AlgoForge.UnitTests/
│       └── AlgoForge.IntegrationTests/
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
```

## Project Status

AlgoForge is under active development.

The current repository contains the core problem-solving, submission, authentication, contest, leaderboard, AI and desktop functionality together with automated testing and deployment infrastructure.

## Author

**Ufuk Çöz**

GitHub: https://github.com/ufukcoz
