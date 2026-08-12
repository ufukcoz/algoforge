AlgoForge Backend

ASP.NET Core Web API backend for the AlgoForge algorithm learning and competitive programming platform.

The AlgoForge backend provides the core application services for authentication, algorithm questions, code submissions, contests, leaderboards, user profiles, categories and AI-assisted learning.

The backend is built with C#, ASP.NET Core, Entity Framework Core and PostgreSQL, following Clean Architecture and CQRS principles.

✨ Features

🔐 JWT-based authentication

👤 User management and profiles

🧩 Algorithm and programming questions

📝 Code submissions

⚡ External code execution with Judge0

🏆 Competitive programming contests

📊 Leaderboards

🗂️ Question categories

🤖 AI-assisted learning

🗄️ PostgreSQL persistence

🛡️ Rate limiting

📖 Swagger / OpenAPI

🐳 Docker support

🔄 CI/CD with GitHub Actions

🌐 Production deployment with Render

🏗️ Architecture

AlgoForge Backend follows a layered architecture based on Clean Architecture principles.

                         ┌─────────────────────┐
                         │    AlgoForge API    │
                         │    ASP.NET Core     │
                         └──────────┬──────────┘
                                    │
                                    ▼
                         ┌─────────────────────┐
                         │     Application     │
                         │    CQRS / Use Cases │
                         └──────────┬──────────┘
                                    │
                                    ▼
                         ┌─────────────────────┐
                         │       Domain        │
                         │   Business Rules    │
                         └─────────────────────┘
                                    ▲
                                    │
                         ┌──────────┴──────────┐
                         │    Infrastructure   │
                         │ EF Core / Services  │
                         └──────────┬──────────┘
                                    │
                                    ▼
                              PostgreSQL

📂 Project Structure

backend/
│
├── src/
│   │
│   ├── AlgoForge.API/
│   │   ├── Controllers/
│   │   ├── Middleware/
│   │   ├── Program.cs
│   │   ├── appsettings.json
│   │   └── AlgoForge.API.csproj
│   │
│   ├── AlgoForge.Application/
│   │   ├── Ai/
│   │   ├── Auth/
│   │   ├── Categories/
│   │   ├── Common/
│   │   ├── Contests/
│   │   ├── Leaderboard/
│   │   ├── Profile/
│   │   ├── Questions/
│   │   ├── Submissions/
│   │   ├── DependencyInjection.cs
│   │   └── AlgoForge.Application.csproj
│   │
│   ├── AlgoForge.Domain/
│   │
│   └── AlgoForge.Infrastructure/
│
├── tests/
│   ├── AlgoForge.UnitTests/
│   └── AlgoForge.IntegrationTests/
│
├── AlgoForge.sln
├── Dockerfile
├── docker-compose.yml
└── README.md

🧱 Architecture Layers

AlgoForge.API

The API layer handles HTTP communication and application configuration.

Responsibilities:

REST controllers

HTTP request/response handling

Middleware

Dependency injection

Authentication configuration

Rate limiting

Swagger / OpenAPI

Application startup

src/AlgoForge.API

AlgoForge.Application

The Application layer contains application use cases and business workflows.

Main feature modules include:

Ai
Auth
Categories
Contests
Leaderboard
Profile
Questions
Submissions

src/AlgoForge.Application

The application layer is organized around features and use cases rather than large centralized service classes.

AlgoForge.Domain

The Domain layer contains core business concepts and business rules.

The domain is designed to remain independent from:

ASP.NET Core

PostgreSQL

Entity Framework Core

AI providers

Judge0

Other external services

src/AlgoForge.Domain

AlgoForge.Infrastructure

The Infrastructure layer contains implementations for persistence and external services.

Responsibilities include:

Entity Framework Core

PostgreSQL

Database configuration

Authentication infrastructure

External API integrations

Persistence implementations

src/AlgoForge.Infrastructure

🔄 CQRS

AlgoForge uses a CQRS-oriented application structure.

Commands modify application state:

Command
   │
   ▼
Command Handler
   │
   ▼
Application Logic
   │
   ▼
Infrastructure

Queries retrieve application data:

Query
   │
   ▼
Query Handler
   │
   ▼
Data Retrieval

This approach helps keep individual use cases isolated, maintainable and testable.

🧩 Application Modules

🔐 Authentication

Authentication is implemented using JWT-based authentication.

Responsibilities include:

User registration

User login

Password hashing

JWT authentication

Token management

Location:

AlgoForge.Application/Auth

🤖 AI

The AI module provides AI-assisted learning functionality.

Location:

AlgoForge.Application/Ai

The AI assistant can support users with:

Understanding programming problems

Receiving hints

Code analysis

Understanding errors

Improving problem-solving approaches

The AI integration is isolated from the core application logic so external providers can be changed with minimal impact.

📝 Questions

The Questions module manages algorithm and programming problems.

Location:

AlgoForge.Application/Questions

Questions are the foundation of the AlgoForge coding practice platform.

💻 Submissions

The Submissions module manages user code submissions.

Location:

AlgoForge.Application/Submissions

User-submitted source code is treated as untrusted input.

The API does not directly execute arbitrary user code inside the ASP.NET Core process.

Instead, code execution is delegated to an external execution environment such as Judge0.

User
 │
 ▼
AlgoForge Desktop
 │
 ▼
AlgoForge API
 │
 ▼
Submission
 │
 ▼
Judge0
 │
 ▼
Execution Result
 │
 ▼
AlgoForge API
 │
 ▼
AlgoForge Desktop

🏆 Contests

The Contests module provides competitive programming functionality.

Location:

AlgoForge.Application/Contests

Contest functionality works together with:

Questions

Submissions

Leaderboards

Users

📊 Leaderboard

The Leaderboard module provides ranking functionality.

Location:

AlgoForge.Application/Leaderboard

👤 Profile

The Profile module provides authenticated user profile functionality.

Location:

AlgoForge.Application/Profile

🗂️ Categories

Categories organize algorithm and programming questions.

Location:

AlgoForge.Application/Categories

🗄️ Database

AlgoForge uses PostgreSQL as its primary relational database.

Entity Framework Core is used for database access and persistence.

Application
     │
     ▼
Infrastructure
     │
     ▼
Entity Framework Core
     │
     ▼
PostgreSQL

🔧 Configuration

Application configuration is managed through the ASP.NET Core configuration system.

Typical configuration values include:

ConnectionStrings
JWT configuration
AI provider configuration
Judge0 configuration
External service configuration

Example environment variable:

ConnectionStrings__DefaultConnection

Sensitive values must never be committed to Git.

For local development, use:

Environment variables

.NET User Secrets

Local configuration

For production, use the hosting platform's environment/secret configuration.

🚀 Local Development

Requirements

Install:

Git

.NET SDK

PostgreSQL

Docker (optional)

Clone

git clone https://github.com/ufukcoz/algoforge.git

Navigate to the backend:

cd algoforge/backend

Restore Dependencies

dotnet restore

Build

dotnet build

Run

dotnet run --project src/AlgoForge.API

🗃️ Entity Framework Core

Install the EF Core CLI if required:

dotnet tool install --global dotnet-ef

Create a migration:

dotnet ef migrations add MigrationName \
  --project src/AlgoForge.Infrastructure \
  --startup-project src/AlgoForge.API

Apply migrations:

dotnet ef database update \
  --project src/AlgoForge.Infrastructure \
  --startup-project src/AlgoForge.API

📡 REST API

The backend exposes REST APIs through ASP.NET Core.

Main API areas include:

Authentication
Questions
Categories
Submissions
Contests
Leaderboard
Profile
AI

Controllers are located under:

src/AlgoForge.API/Controllers

📖 Swagger / OpenAPI

Swagger/OpenAPI is available during development.

After starting the API:

https://localhost:{port}/swagger

Swagger provides:

Endpoint documentation

Request models

Response models

Authentication testing

Interactive API testing

🛡️ Security

Security is an important part of AlgoForge because the backend handles authentication data and untrusted source code.

Current security mechanisms include:

JWT authentication

Password hashing

Rate limiting

Centralized exception handling

Environment-based secrets

External code execution

Docker-based deployment

🔒 Untrusted Code Execution

User-submitted source code must never be executed directly inside the API process.

User Code
    │
    ▼
AlgoForge API
    │
    ▼
External Execution Environment
    │
    ▼
Execution Result

This architecture helps isolate potentially dangerous code execution from the main application.

⏱️ Rate Limiting

Rate limiting helps protect the API against excessive requests.

It is especially important for:

Authentication

Code submissions

AI requests

Resource-intensive operations

Future improvements include:

Per-user quotas

Per-endpoint limits

AI usage limits

Submission limits

🧪 Testing

Backend tests are organized separately:

tests/
├── AlgoForge.UnitTests/
└── AlgoForge.IntegrationTests/

Run tests:

dotnet test

Build the solution:

dotnet build

Testing coverage will continue to expand as the project evolves.

🐳 Docker

The backend supports Docker-based deployment.

backend/
├── Dockerfile
└── docker-compose.yml

Build the Docker image:

docker build -t algoforge-api .

Run the container:

docker run -p 8080:8080 algoforge-api

For local multi-container development:

docker compose up --build

Production secrets are supplied through environment configuration.

🌐 Production Deployment

The AlgoForge backend is deployed using a containerized production architecture.

Infrastructure

Service

Purpose

GitHub

Source code

GitHub Actions

CI/CD

GitHub Container Registry

Docker image registry

Render

Production API hosting

PostgreSQL

Production database

Docker

Containerization

Judge0

Code execution

AI Provider

AI services

Deployment Flow

Developer
    │
    ▼
GitHub Repository
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
            │
            ▼
AlgoForge ASP.NET Core API
            │
            ├───────────────┐
            ▼               ▼
       PostgreSQL       External Services
                        ├── Judge0
                        └── AI Provider

The production backend runs as a Docker-based ASP.NET Core application on Render.

The repository contains:

render.yaml

which keeps the Render deployment configuration version-controlled.

🔐 Production Secrets

Production secrets are never stored in the repository.

Examples:

Database credentials
JWT secrets
AI API keys
Judge0 credentials
External service credentials

These values are supplied through Render environment configuration.

Never commit secrets to Git.

🔄 CI/CD

AlgoForge uses GitHub Actions for automated workflows.

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

The CI/CD pipeline will continue to evolve as automated testing and security checks are expanded.

🗺️ Roadmap

Authentication

Refresh token rotation

Session management

Logout / revoke sessions

Email verification

Password reset

Authentication auditing

Security

Production CORS policy

Advanced submission abuse protection

AI usage quotas

Submission quotas

Security-focused integration tests

Improved audit logging

Platform

Advanced user statistics

Learning progress tracking

Question recommendation system

Achievement system

Streak system

Advanced contest functionality

Administration

Admin API

User management

Question management

Test-case management

Contest management

Submission monitoring

System health endpoints

🧭 Design Principles

Separation of Concerns

Each architectural layer has a clearly defined responsibility.

Dependency Inversion

Core application logic should not depend directly on infrastructure implementations.

Testability

Application use cases should be independently testable.

Security by Design

User input, credentials and source code are treated as potentially sensitive or untrusted.

Maintainability

Feature-oriented modules are preferred over large centralized service classes.

Scalability

The architecture allows individual modules and infrastructure services to evolve independently.

🔗 Related Components

AlgoForge
│
├── Backend
│   └── ASP.NET Core API
│
├── Desktop
│   └── Electron + React + TypeScript
│
└── Infrastructure
    ├── PostgreSQL
    ├── Judge0
    ├── AI Provider
    └── Render

Desktop client:

../desktop

👨‍💻 Author

Ufuk Çöz

Software Engineering Student & Developer

GitHub:

https://github.com/ufukcoz

📄 License

The project's licensing model has not yet been finalized.

A formal open-source license will be added before official open-source distribution.

📌 Status

Project: AlgoForge

Component: Backend API

Framework: ASP.NET Core

Language: C#

Database: PostgreSQL

ORM: Entity Framework Core

Architecture: Clean Architecture + CQRS

Authentication: JWT

Code Execution: Judge0

AI: AI Provider

Containerization: Docker

CI/CD: GitHub Actions

Container Registry: GitHub Container Registry

Production Hosting: Render

Status: Active Development
