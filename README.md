# AlgoForge

> AI-powered algorithm learning and competitive programming platform built with ASP.NET Core, React, TypeScript, PostgreSQL, Judge0 and Electron.

AlgoForge is a desktop-oriented algorithm learning platform designed to help developers improve their problem-solving and programming skills through coding challenges, code execution, contests, leaderboards and AI-assisted learning.

The project combines a modular ASP.NET Core backend with an Electron-based desktop client and a PostgreSQL-powered persistence layer.

---

## ✨ Features

### 🧩 Algorithm Practice

* Browse programming and algorithm questions
* Difficulty-based questions
* Category-based organization
* Question detail pages
* Monaco-based code editor
* Code execution and submissions
* Submission results
* Supported programming languages

### 🤖 AI Assistant

AlgoForge includes an AI-powered assistant designed to support the learning process.

The assistant can help users with:

* Understanding programming problems
* Getting hints
* Understanding errors
* Analyzing code
* Improving problem-solving approaches

The goal is to use AI as a learning assistant rather than simply providing solutions.

### 🏆 Contests

AlgoForge includes a competitive programming contest system.

* Contest listing
* Contest details
* Contest questions
* Contest submissions
* Contest leaderboard

### 📊 Leaderboard

Users can compare their competitive programming performance through leaderboard functionality.

### 👤 User Profiles

Authenticated users can access their profile and account-related information.

### 💻 Desktop Application

AlgoForge is distributed as a desktop application built with Electron.

The desktop client provides:

* React-based interface
* TypeScript
* Monaco Editor
* Electron
* API integration
* Authentication
* Coding environment
* AI assistant integration

---

# 🏗️ System Architecture

AlgoForge is composed of a desktop client, ASP.NET Core backend, PostgreSQL database and external services.

```text
                           ┌──────────────────────┐
                           │   AlgoForge Desktop  │
                           │ Electron + React     │
                           │ TypeScript            │
                           └───────────┬──────────┘
                                       │
                                       │ HTTP / REST
                                       ▼
                           ┌──────────────────────┐
                           │    AlgoForge API     │
                           │    ASP.NET Core      │
                           └───────────┬──────────┘
                                       │
                                       ▼
                           ┌──────────────────────┐
                           │     Application      │
                           │   CQRS / Use Cases   │
                           ├──────────────────────┤
                           │ Auth                 │
                           │ Questions            │
                           │ Submissions          │
                           │ Contests             │
                           │ Leaderboard          │
                           │ Profile              │
                           │ Categories           │
                           │ AI                   │
                           └───────────┬──────────┘
                                       │
                       ┌───────────────┼───────────────┐
                       ▼               ▼               ▼
                ┌─────────────┐ ┌─────────────┐ ┌─────────────┐
                │ PostgreSQL  │ │   Judge0    │ │ AI Provider │
                │  Database   │ │ Code Engine │ │ AI Services │
                └─────────────┘ └─────────────┘ └─────────────┘
```

---

# 🧱 Backend Architecture

The backend follows Clean Architecture principles and uses a modular application structure.

```text
backend/
│
├── src/
│   ├── AlgoForge.API/
│   ├── AlgoForge.Application/
│   ├── AlgoForge.Domain/
│   └── AlgoForge.Infrastructure/
│
└── tests/
    ├── AlgoForge.UnitTests/
    └── AlgoForge.IntegrationTests/
```

### Main architectural concepts

* Clean Architecture
* CQRS
* Dependency Injection
* Entity Framework Core
* PostgreSQL
* JWT Authentication
* REST API
* Rate Limiting
* Centralized Exception Handling
* Automated Testing
* Docker-based deployment

---

# 🖥️ Desktop Architecture

The desktop application is built using Electron, React and TypeScript.

```text
desktop/
│
├── src/
│   ├── components/
│   ├── contexts/
│   ├── pages/
│   ├── services/
│   └── ...
│
├── electron/
├── public/
└── package.json
```

The desktop client communicates with the backend through HTTP APIs.

PostgreSQL, Judge0 and AI services are accessed through the backend rather than directly from the desktop application.

---

# 🛠️ Technology Stack

## Backend

| Technology            | Purpose                  |
| --------------------- | ------------------------ |
| C#                    | Programming language     |
| ASP.NET Core          | REST API                 |
| Entity Framework Core | ORM / data access        |
| PostgreSQL            | Relational database      |
| JWT                   | Authentication           |
| CQRS                  | Application architecture |
| Clean Architecture    | Backend architecture     |
| Swagger / OpenAPI     | API documentation        |

## Desktop

| Technology       | Purpose               |
| ---------------- | --------------------- |
| Electron         | Desktop runtime       |
| React            | User interface        |
| TypeScript       | Type-safe development |
| Vite             | Frontend tooling      |
| Monaco Editor    | Code editor           |
| Electron Builder | Application packaging |

## External Services

| Service     | Purpose                     |
| ----------- | --------------------------- |
| Judge0      | User code execution         |
| AI Provider | AI-assisted learning        |
| PostgreSQL  | Persistent application data |
| Render      | Production API hosting      |

## DevOps

* Docker
* GitHub Actions
* GitHub Container Registry
* Render

---

# 📂 Project Structure

```text
AlgoForge/
│
├── backend/
│   │
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
│   ├── electron/
│   ├── public/
│   └── package.json
│
├── .github/
│   └── workflows/
│
├── questions.json
├── render.yaml
├── .gitignore
└── README.md
```

---

# 🚀 Getting Started

## Prerequisites

Install the following tools before running AlgoForge locally:

* Git
* .NET SDK
* Node.js
* npm
* PostgreSQL
* Docker (optional)

---

# ⚙️ Backend Setup

Clone the repository:

```bash
git clone https://github.com/ufukcoz/algoforge.git
```

Navigate to the backend:

```bash
cd algoforge/backend
```

Restore dependencies:

```bash
dotnet restore
```

Build the backend:

```bash
dotnet build
```

Run the API:

```bash
dotnet run --project src/AlgoForge.API
```

The API will run using the ASP.NET Core configuration for the selected environment.

---

# 🗄️ Database

AlgoForge uses PostgreSQL as its primary database.

The database connection is configured through the ASP.NET Core configuration system.

Typical configuration:

```text
ConnectionStrings__DefaultConnection
```

For local development, database credentials should be supplied through environment variables, User Secrets or local configuration.

Never commit production database credentials to Git.

---

# 🖥️ Desktop Setup

Navigate to the desktop application:

```bash
cd desktop
```

Install dependencies:

```bash
npm install
```

Start the Vite development server:

```bash
npm run dev
```

Run the Electron development environment:

```bash
npm run electron:dev
```

---

# 📦 Desktop Production Build

Build the frontend:

```bash
npm run build
```

Build the Electron application:

```bash
npm run electron:build
```

Production artifacts are generated in the configured release directory.

---

# 🐳 Docker

The backend can be containerized using Docker.

Build the backend image:

```bash
docker build -t algoforge-api .
```

Run the container:

```bash
docker run -p 8080:8080 algoforge-api
```

For local multi-container development:

```bash
docker compose up --build
```

Production secrets should be supplied through environment configuration rather than being embedded in Docker images.

---

# 🌐 Production Deployment

AlgoForge uses a containerized production architecture.

## Production Infrastructure

| Service                   | Purpose                  |
| ------------------------- | ------------------------ |
| GitHub                    | Source code repository   |
| GitHub Actions            | CI/CD automation         |
| GitHub Container Registry | Container image registry |
| Render                    | Production API hosting   |
| PostgreSQL                | Production database      |
| Docker                    | Backend containerization |

### Deployment Flow

```text
Developer
    │
    ▼
GitHub Repository
    │
    ▼
GitHub Actions
    │
    ├── Build
    ├── Test
    └── Docker Build
            │
            ▼
GitHub Container Registry
            │
            ▼
         Render
            │
            ▼
AlgoForge ASP.NET Core API
            │
            ├───────────────┐
            ▼               ▼
       PostgreSQL       External Services
                        ├── Judge0
                        └── AI Provider
```

The AlgoForge backend is deployed as a Docker-based ASP.NET Core application on **Render**.

The repository contains:

```text
render.yaml
```

which keeps the Render deployment configuration version-controlled alongside the application source code.

---

# 🔐 Production Configuration

Production secrets are not stored in the repository.

Examples include:

* PostgreSQL connection string
* JWT secrets
* AI provider credentials
* Judge0 credentials
* Other service credentials

These values are supplied through the production environment configuration.

Sensitive credentials must never be committed to Git.

---

# 🧪 Testing

The backend contains separate testing projects:

```text
backend/tests/
├── AlgoForge.UnitTests/
└── AlgoForge.IntegrationTests/
```

Run all backend tests:

```bash
dotnet test
```

Build the backend:

```bash
dotnet build
```

Testing coverage will continue to expand as the platform evolves.

---

# 📡 API

The backend exposes a REST API through ASP.NET Core.

Major API areas include:

```text
Authentication
Questions
Categories
Submissions
Contests
Leaderboard
Profile
AI
```

The API implementation is located under:

```text
backend/src/AlgoForge.API
```

---

# 📖 Swagger / OpenAPI

During development, Swagger/OpenAPI is available through the ASP.NET Core API.

After starting the backend, open:

```text
https://localhost:{port}/swagger
```

Swagger can be used to:

* Explore endpoints
* Inspect request models
* Inspect response models
* Test API operations
* Test authenticated endpoints

---

# 🔒 Security

Security is a major consideration because AlgoForge processes authentication data and user-submitted source code.

Current security-related mechanisms include:

* JWT authentication
* Password hashing
* API rate limiting
* Centralized exception handling
* Environment-based secrets
* External code execution architecture
* Docker-based backend deployment

### Untrusted Code

User-submitted source code should never be executed directly inside the AlgoForge API process.

Code execution is delegated to an external execution environment such as Judge0.

This separation reduces the risk associated with executing untrusted source code inside the main application.

---

# ⏱️ Rate Limiting

The API includes rate-limiting mechanisms to protect backend resources.

Rate limiting is particularly important for:

* Authentication
* Code submissions
* AI requests
* Resource-intensive operations

Future improvements include more granular per-user and per-operation quotas.

---

# 🔄 CI/CD

AlgoForge uses GitHub Actions for automated development workflows.

The CI/CD architecture is designed around:

```text
Git Push / Pull Request
          │
          ▼
     GitHub Actions
          │
          ├── Restore
          ├── Build
          ├── Test
          └── Docker Build
                    │
                    ▼
          GitHub Container Registry
                    │
                    ▼
                 Render
```

The CI/CD pipeline will continue to evolve as additional testing and security checks are introduced.

---

# 🗺️ Roadmap

## 🔐 Authentication & Security

* [ ] Refresh token rotation
* [ ] Secure Electron token storage
* [ ] Session management
* [ ] Logout from all sessions
* [ ] Email verification
* [ ] Password reset
* [ ] Production CORS policy
* [ ] Advanced submission abuse protection
* [ ] AI usage quotas
* [ ] Security audit logging

## 🧪 Testing

* [ ] Expand unit test coverage
* [ ] Expand integration test coverage
* [ ] Authentication test suite
* [ ] Submission test suite
* [ ] Contest test suite
* [ ] Rate-limit tests
* [ ] Security-focused integration tests
* [ ] CI test enforcement

## 📊 User Experience

* [ ] Advanced user statistics
* [ ] Learning progress tracking
* [ ] Daily challenge
* [ ] Streak system
* [ ] Achievement system
* [ ] Advanced question filtering
* [ ] Pagination improvements

## 🤖 AI

* [ ] Improved AI tutor
* [ ] Hint system
* [ ] Code review mode
* [ ] Complexity analysis
* [ ] AI usage quotas
* [ ] AI usage analytics
* [ ] Personalized question recommendations

## 🏆 Competitive Programming

* [ ] Advanced contest functionality
* [ ] Improved contest rankings
* [ ] Contest statistics
* [ ] Submission analytics
* [ ] Anti-cheat mechanisms

## 🛠️ Administration

* [ ] Admin dashboard
* [ ] User management
* [ ] Question management
* [ ] Test-case management
* [ ] Contest management
* [ ] Submission monitoring
* [ ] AI usage monitoring
* [ ] System health monitoring

## 🖥️ Desktop

* [ ] Secure credential storage
* [ ] Automatic application updates
* [ ] GitHub Releases integration
* [ ] Improved offline handling
* [ ] Connection status UI
* [ ] Crash reporting

---

# 🎯 Project Goals

AlgoForge aims to become more than a traditional coding challenge application.

The long-term vision is to combine:

```text
Algorithm Practice
        +
Competitive Programming
        +
AI-assisted Learning
        +
Personalized Progress
        +
Developer Analytics
```

into a single developer-focused learning platform.

---

# 🧠 Development Philosophy

AlgoForge is being developed around several core principles.

### Clean Architecture

Keep business logic independent from infrastructure and external services.

### Separation of Concerns

Each application module should have a clearly defined responsibility.

### Testability

Application logic should be independently testable.

### Security by Design

User input, credentials and submitted source code are treated as potentially sensitive or untrusted.

### Maintainability

Prefer modular and readable implementations over unnecessary complexity.

### Scalability

The architecture should allow individual services and modules to evolve without requiring a complete rewrite.

---

# 🤝 Contributing

Contributions, suggestions and improvements are welcome.

### Development workflow

1. Fork the repository.
2. Create a feature branch.
3. Implement the change.
4. Add or update tests.
5. Verify the build.
6. Open a pull request.

Example:

```bash
git checkout -b feature/my-feature

git add .

git commit -m "feat: add my feature"

git push origin feature/my-feature
```

---

# 📚 Documentation

Additional documentation will be maintained under the `docs` directory.

Planned documentation includes:

```text
docs/
├── ARCHITECTURE.md
├── API.md
└── DEVELOPMENT.md
```

Security-related information will be documented separately in:

```text
SECURITY.md
```

---

# 📄 License

The project's licensing model has not yet been finalized.

A formal open-source license will be added before official open-source distribution.

---

# 👨‍💻 Author

**Ufuk Çöz**

Software Engineering Student & Developer

GitHub:

https://github.com/ufukcoz

---

# ⭐ Support the Project

If you find AlgoForge interesting, consider giving the repository a ⭐ on GitHub.

AlgoForge is actively evolving toward a production-ready algorithm learning and competitive programming platform.

---

## 📌 Project Status

**Status:** Active Development

**Backend:** ASP.NET Core

**Desktop:** Electron + React + TypeScript

**Database:** PostgreSQL

**Code Execution:** Judge0

**AI:** AI Provider

**Containerization:** Docker

**CI/CD:** GitHub Actions

**Container Registry:** GitHub Container Registry

**Production Hosting:** Render
