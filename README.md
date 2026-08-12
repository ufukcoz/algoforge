# AlgoForge

> AI-powered algorithm learning and coding practice platform built with ASP.NET Core, React, TypeScript, PostgreSQL, Judge0 and Electron.

AlgoForge is a desktop-oriented algorithm learning platform designed to help developers improve their problem-solving and programming skills through coding challenges, real-time code execution, contests, leaderboards and AI-assisted learning.

The project is built with a modular backend architecture and a modern Electron + React desktop client.

---

## ✨ Features

### 🧩 Algorithm Practice

* Browse algorithm and programming questions
* Difficulty-based questions
* Category-based organization
* Question detail pages
* Code editor powered by Monaco Editor
* Multi-language code execution
* Submission history
* Execution results

### 🤖 AI Assistant

AlgoForge includes an AI-powered assistant designed to support the learning process.

The assistant can be used for:

* Understanding programming problems
* Getting hints
* Analyzing code
* Understanding errors
* Improving problem-solving approaches

The goal is to use AI as a learning assistant rather than simply providing solutions.

### 🏆 Contests

AlgoForge includes a contest system for competitive programming scenarios.

* Contest listing
* Contest details
* Contest questions
* Submissions
* Contest leaderboard

### 📊 Leaderboard

Users can compare their performance through leaderboard functionality.

### 👤 User Profiles

The platform includes user profile functionality for managing account-related information and programming activity.

### 💻 Desktop Application

AlgoForge is distributed as a desktop application using Electron.

The desktop client provides:

* React-based UI
* TypeScript
* Monaco Editor
* Electron
* API integration
* Authentication
* Coding environment

---

# 🏗️ Architecture

AlgoForge follows a modular backend architecture based on **Clean Architecture principles**.

```text
                         ┌─────────────────────┐
                         │   AlgoForge Desktop  │
                         │ Electron + React    │
                         │ TypeScript           │
                         └──────────┬──────────┘
                                    │
                                    │ HTTP / REST
                                    ▼
                         ┌─────────────────────┐
                         │    AlgoForge API    │
                         │    ASP.NET Core     │
                         └──────────┬──────────┘
                                    │
                                    ▼
                         ┌─────────────────────┐
                         │    Application      │
                         │ CQRS / Use Cases    │
                         ├─────────────────────┤
                         │ Auth                │
                         │ Questions           │
                         │ Submissions         │
                         │ Contests            │
                         │ Leaderboard         │
                         │ Profile             │
                         │ AI                  │
                         └──────────┬──────────┘
                                    │
                                    ▼
                         ┌─────────────────────┐
                         │       Domain        │
                         │   Business Rules    │
                         └──────────┬──────────┘
                                    │
                                    ▼
                         ┌─────────────────────┐
                         │ Infrastructure      │
                         │ EF Core / Services  │
                         └──────────┬──────────┘
                                    │
                   ┌────────────────┼────────────────┐
                   ▼                ▼                ▼
             PostgreSQL          Judge0          AI Provider
```

---

# 🧱 Backend Architecture

The backend is organized around Clean Architecture principles.

```text
backend/
│
├── src/
│   ├── AlgoForge.API/
│   │
│   ├── AlgoForge.Application/
│   │   ├── Ai/
│   │   ├── Auth/
│   │   ├── Categories/
│   │   ├── Contests/
│   │   ├── Leaderboard/
│   │   ├── Profile/
│   │   ├── Questions/
│   │   └── Submissions/
│   │
│   ├── AlgoForge.Domain/
│   │
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
* Centralized exception handling
* Rate limiting
* Automated testing

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
│
├── public/
│
└── package.json
```

### Technologies

* Electron
* React
* TypeScript
* Vite
* Monaco Editor
* Electron Builder

---

# 🛠️ Tech Stack

## Backend

| Technology            | Purpose                  |
| --------------------- | ------------------------ |
| C#                    | Programming language     |
| ASP.NET Core          | Web API                  |
| Entity Framework Core | ORM                      |
| PostgreSQL            | Database                 |
| JWT                   | Authentication           |
| CQRS                  | Application architecture |
| Clean Architecture    | System architecture      |
| Swagger / OpenAPI     | API documentation        |

## Frontend / Desktop

| Technology       | Purpose                        |
| ---------------- | ------------------------------ |
| React            | UI                             |
| TypeScript       | Type-safe frontend development |
| Vite             | Frontend tooling               |
| Electron         | Desktop application            |
| Monaco Editor    | Code editor                    |
| Electron Builder | Application packaging          |

## External Services

| Service     | Purpose                 |
| ----------- | ----------------------- |
| Judge0      | Code execution          |
| AI Provider | AI-assisted learning    |
| PostgreSQL  | Persistent data storage |

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

Before running AlgoForge locally, install:

* .NET SDK
* Node.js
* npm
* PostgreSQL
* Git
* Docker (optional)

---

# ⚙️ Backend Setup

Clone the repository:

```bash
git clone https://github.com/ufukcoz/algoforge.git

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

The API will be available according to the configured ASP.NET Core environment and launch settings.

Swagger can be used to explore the available API endpoints during development.

---

# 🗄️ Database

AlgoForge uses PostgreSQL as its primary relational database.

Configure the database connection through environment-specific configuration.

Example:

```text
ConnectionStrings__DefaultConnection
```

Do not commit real database credentials to the repository.

---

# 🖥️ Desktop Setup

Navigate to the desktop project:

```bash
cd desktop
```

Install dependencies:

```bash
npm install
```

Start the development environment:

```bash
npm run dev
```

For Electron development:

```bash
npm run electron:dev
```

---

# 📦 Build Desktop Application

Create a production build:

```bash
npm run build
```

Build the Electron application:

```bash
npm run electron:build
```

The generated application artifacts are placed in the configured release directory.

---

# 🐳 Docker

The backend can be containerized using Docker.

Build the image:

```bash
docker build -t algoforge-api .
```

Run the container:

```bash
docker run -p 8080:8080 algoforge-api
```

For production deployments, environment variables should be provided through the hosting platform rather than hardcoded in the image.

---

# 🔐 Security

Security is an important part of AlgoForge because the platform executes user-submitted source code.

Current security-related components include:

* JWT authentication
* API rate limiting
* Centralized exception handling
* Environment-based configuration
* Containerized backend deployment
* External code execution through Judge0

### Security considerations

User-submitted code should never be executed directly inside the AlgoForge API process.

Code execution is delegated to the Judge0 execution environment.

Sensitive values such as:

* Database credentials
* JWT secrets
* AI API keys
* Judge0 credentials

must be stored using environment variables or secure secret management.

For security-related issues, see:

`SECURITY.md`

---

# 🧪 Testing

The backend contains separate test projects:

```text
backend/tests/
├── AlgoForge.UnitTests/
└── AlgoForge.IntegrationTests/
```

Run backend tests with:

```bash
dotnet test
```

Testing is an ongoing part of the project's development and will continue to expand as new application features are introduced.

---

# 🔄 CI/CD

AlgoForge uses GitHub Actions for automated development workflows.

The CI pipeline is responsible for tasks such as:

```text
Push / Pull Request
        │
        ▼
Restore dependencies
        │
        ▼
Build backend
        │
        ▼
Build desktop
        │
        ▼
Build Docker image
        │
        ▼
Publish container
```

The project uses GitHub Container Registry for container images and supports container-based deployment.

---

# 🌐 Deployment

The backend is designed to run as a containerized ASP.NET Core application.

The repository includes deployment configuration for Render.

Production deployments should provide secrets through environment configuration rather than committing credentials to source control.

---

# 🗺️ Roadmap

AlgoForge is actively evolving.

### 🔐 Security

* [ ] Refresh token rotation
* [ ] Secure Electron token storage
* [ ] Session management
* [ ] Email verification
* [ ] Password reset
* [ ] Production CORS policy
* [ ] Advanced code-execution abuse protection

### 🧪 Testing

* [ ] Expand unit test coverage
* [ ] Expand integration test coverage
* [ ] Authentication tests
* [ ] Submission tests
* [ ] Contest tests
* [ ] CI test enforcement

### 📊 User Experience

* [ ] Advanced user statistics
* [ ] Learning progress tracking
* [ ] Streak system
* [ ] Achievement system
* [ ] Advanced question filtering
* [ ] Pagination improvements

### 🤖 AI

* [ ] Improved AI tutoring flow
* [ ] Hint system
* [ ] Code review mode
* [ ] AI usage quotas
* [ ] AI usage analytics
* [ ] Personalized question recommendations

### 🛠️ Administration

* [ ] Admin dashboard
* [ ] User management
* [ ] Question management
* [ ] Test case management
* [ ] Contest management
* [ ] Submission monitoring
* [ ] System health monitoring

### 🚀 Desktop

* [ ] Secure credential storage
* [ ] Automatic application updates
* [ ] GitHub Releases integration
* [ ] Improved offline/connection handling

---

# 🎯 Project Goals

AlgoForge aims to become more than a traditional coding challenge application.

The long-term goal is to combine:

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

# 🤝 Contributing

Contributions, suggestions and improvements are welcome.

Before contributing:

1. Fork the repository.
2. Create a feature branch.
3. Implement your changes.
4. Add or update tests where appropriate.
5. Open a pull request.

Example:

```bash
git checkout -b feature/my-feature

git add .

git commit -m "feat: add my feature"

git push origin feature/my-feature
```

---

# 📜 License

This project currently does not define a finalized open-source license.

A license will be added before the project is officially distributed as an open-source project.

---

# 👨‍💻 Author

**Ufuk Çöz**

Software Engineering Student & Developer

GitHub: [@ufukcoz](https://github.com/ufukcoz)

---

# ⭐ Support

If you find AlgoForge interesting, consider giving the repository a ⭐ on GitHub.

The project is actively evolving and new features, security improvements and architectural improvements are continuously being developed.
