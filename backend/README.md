# AlgoForge Backend

Backend API of the AlgoForge algorithm learning and competitive programming platform.

Built with **ASP.NET Core 8**, **Entity Framework Core** and **PostgreSQL**.

## Architecture

The backend follows a layered architecture:

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

### Projects

```text
backend/
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
```

## Main Modules

The backend currently contains functionality for:

* Authentication
* User registration and login
* JWT access tokens
* Refresh tokens
* Refresh-token rotation and reuse detection
* Questions
* Categories
* Submissions
* Contests
* Contest participants
* Contest leaderboard
* Global leaderboard
* Profile
* AI assistant

## Security

Authentication and authorization include:

* JWT Bearer authentication
* Role-based authorization
* Admin authorization for question management
* BCrypt password hashing
* SHA-256 refresh-token hashing
* Refresh-token rotation
* Refresh-token family tracking
* Refresh-token reuse detection
* Private contest authorization
* Submission ownership checks

API hardening includes:

* Global rate limiting
* Authentication-specific rate limiting
* Rate limiting for expensive endpoints
* Security response headers
* Centralized exception handling
* Health endpoint

## Technologies

* C#
* ASP.NET Core 8
* Entity Framework Core
* PostgreSQL
* MediatR
* JWT Authentication
* BCrypt
* Docker

## Database

AlgoForge uses PostgreSQL.

Entity Framework Core migrations are used to manage the database schema.

Current migrations include the initial schema together with question, submission, contest, authentication, email verification, user role, refresh-token hashing and refresh-token family changes.

Database credentials and production secrets should be supplied through environment configuration rather than committed to source control.

## Configuration

Production configuration uses environment variables for sensitive values such as:

* JWT secret
* Gemini API key
* Resend API key
* PostgreSQL connection string

## Run Locally

```bash
cd backend

dotnet restore
dotnet build

dotnet run --project src/AlgoForge.API
```

## Docker

Build the API image:

```bash
docker build -t algoforge-api .
```

Local development can also use Docker Compose for PostgreSQL and the API.

```bash
docker compose up -d
```

## Health Check

The API exposes:

```text
GET /health
```

The endpoint checks database connectivity and is used as the production service health check.

## Tests

Tests are located under:

```text
tests/
├── AlgoForge.UnitTests/
└── AlgoForge.IntegrationTests/
```

Run all backend tests:

```bash
dotnet test
```

The test suite covers authentication, refresh tokens, authorization, contest access, leaderboard access, submissions, rate limiting, security headers, CORS and health checks.

## Production

The backend is deployed on Render using Docker.

```text
Render
   │
   ▼
ASP.NET Core API
   │
   ▼
PostgreSQL
```
