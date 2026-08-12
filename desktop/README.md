# AlgoForge Desktop

> Desktop client for the AlgoForge algorithm learning and competitive programming platform.

AlgoForge Desktop is a cross-platform desktop application built with **Electron, React and TypeScript**.

It provides the user-facing interface for solving algorithm problems, writing code, submitting solutions, participating in contests and interacting with the AlgoForge backend and AI assistant.

---

# ✨ Features

## 🧩 Algorithm Practice

* Browse programming questions
* View question details
* Read problem descriptions
* View constraints and examples
* Select programming language
* Write and edit source code
* Submit solutions
* View execution results

---

## 💻 Monaco Code Editor

AlgoForge uses **Monaco Editor** as its integrated coding environment.

The editor provides a familiar developer experience with features such as:

* Syntax highlighting
* Code editing
* Programming language support
* Editor shortcuts
* Developer-oriented editing experience

---

## 🤖 AI Assistant

The desktop client includes an AI assistant integrated with the AlgoForge backend.

The AI assistant can be used during the learning process for:

* Understanding problems
* Getting hints
* Understanding errors
* Code analysis
* Problem-solving guidance

The AI assistant is designed to support learning rather than simply replacing the problem-solving process.

---

## 🏆 Contests

The desktop client provides contest-related functionality.

Current contest-related screens include:

* Contest list
* Contest details
* Contest questions
* Contest leaderboard

---

## 📊 Leaderboard

Users can view competitive programming rankings through the leaderboard interface.

---

## 👤 Profile

The desktop application includes user profile functionality for authenticated users.

---

# 🏗️ Architecture

The desktop application follows a typical Electron architecture.

```text id="l4h8ab"
                    AlgoForge Desktop
                           │
            ┌──────────────┴──────────────┐
            │                             │
            ▼                             ▼
      Electron Main                 Renderer Process
            │                             │
            │                       React + TypeScript
            │                             │
            ▼                             ▼
       Electron APIs                UI Components
            │                       Pages / Contexts
            │                             │
            └──────────────┬──────────────┘
                           │
                           ▼
                    AlgoForge REST API
                           │
                           ▼
                    ASP.NET Core Backend
```

---

# 📂 Project Structure

```text id="w1c9af"
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
├── package.json
├── package-lock.json
├── vite.config.*
└── README.md
```

The exact structure may evolve as the application grows.

---

# 🛠️ Technology Stack

| Technology       | Purpose                       |
| ---------------- | ----------------------------- |
| Electron         | Desktop application runtime   |
| React            | User interface                |
| TypeScript       | Type-safe development         |
| Vite             | Development and build tooling |
| Monaco Editor    | Code editor                   |
| Electron Builder | Application packaging         |

---

# 🔌 Backend Communication

The desktop client communicates with the AlgoForge ASP.NET Core backend through HTTP APIs.

```text id="t6b9ro"
AlgoForge Desktop
       │
       │ HTTP / REST
       ▼
AlgoForge API
       │
       ├── Authentication
       ├── Questions
       ├── Submissions
       ├── Contests
       ├── Leaderboard
       ├── Profile
       └── AI
```

The API base URL should be configured through the application's environment/configuration rather than hardcoded into production builds.

---

# 🔐 Authentication

The desktop application communicates with the backend authentication system.

The authentication flow is conceptually:

```text id="xg0e4k"
User
 │
 ▼
Login
 │
 ▼
AlgoForge API
 │
 ▼
JWT Authentication
 │
 ▼
Authenticated Desktop Session
```

Authentication security is an active area of development.

Future improvements include:

* Secure refresh-token storage
* Refresh-token rotation
* Session management
* Logout from all devices
* Improved credential protection

---

# 🌐 Production Environment

The desktop application communicates with the production AlgoForge backend.

The current production architecture is:

```text id="w5k3d2"
AlgoForge Desktop
       │
       ▼
Render
       │
       ▼
AlgoForge ASP.NET Core API
       │
       ├──────────────┐
       ▼              ▼
 PostgreSQL        External Services
                  ├── Judge0
                  └── AI Provider
```

The desktop client does not directly connect to PostgreSQL.

All application data access goes through the backend API.

---

# 🚀 Getting Started

## Requirements

Install:

* Node.js
* npm
* Git

Optional:

* Visual Studio Code
* Electron development tools

---

# 📥 Installation

Clone the repository:

```bash id="2v0e8q"
git clone https://github.com/ufukcoz/algoforge.git
```

Navigate to the desktop application:

```bash id="t5w7gk"
cd algoforge/desktop
```

Install dependencies:

```bash id="t4p9my"
npm install
```

---

# 🧑‍💻 Development

Start the Vite development server:

```bash id="t7c1m5"
npm run dev
```

Run the Electron development environment:

```bash id="y5f8j4"
npm run electron:dev
```

---

# 📦 Production Build

Build the React application:

```bash id="v3u8z2"
npm run build
```

Build the Electron application:

```bash id="m8s5kp"
npm run electron:build
```

The generated production artifacts are placed in the configured release directory.

---

# 🪟 Windows Build

AlgoForge uses Electron Builder for desktop application packaging.

The Windows build generates an installer suitable for distributing the application to end users.

The production build configuration is maintained in:

```text id="h4w9k1"
package.json
```

---

# 🔄 Release Process

The planned release workflow is:

```text id="j3n7xq"
Code Change
    │
    ▼
Git Commit
    │
    ▼
GitHub
    │
    ▼
CI Build
    │
    ▼
Electron Build
    │
    ▼
GitHub Release
    │
    ▼
AlgoForge Installer
```

Future releases will use semantic versioning:

```text id="v7x2k4"
MAJOR.MINOR.PATCH
```

Example:

```text id="k8f2q1"
1.0.0
1.1.0
1.1.1
2.0.0
```

---

# 🔒 Security

The desktop application handles authentication credentials and communicates with the production API.

Security considerations include:

* Never hardcode API secrets
* Never store database credentials in the desktop application
* Do not expose backend service credentials to the renderer
* Validate API responses
* Treat user-submitted code as untrusted
* Use secure credential storage for persistent tokens

### Planned Security Improvements

* [ ] Secure OS credential storage
* [ ] Refresh token rotation
* [ ] Session management
* [ ] Improved Electron IPC isolation
* [ ] Production security audit
* [ ] Automatic update signature verification

---

# 🌐 Connection Handling

Because AlgoForge Desktop depends on a remote API, the application should gracefully handle:

* No internet connection
* API unavailable
* Authentication expiration
* Server errors
* Request timeout
* Render cold-start delays

The planned UX is:

```text id="x9k4e2"
API Available
     │
     ▼
Normal Application

API Unavailable
     │
     ▼
Connection Error
     │
     ▼
Retry
```

---

# 🧪 Testing

Frontend and desktop testing will be expanded as the application grows.

Planned areas include:

* Component tests
* Authentication tests
* Question page tests
* Submission flow tests
* Contest tests
* API integration tests
* Electron integration tests

---

# 🗺️ Roadmap

## Authentication

* [ ] Secure token storage
* [ ] Refresh token rotation
* [ ] Session management
* [ ] Logout all sessions

## Coding Experience

* [ ] Improved editor settings
* [ ] Better execution feedback
* [ ] Submission history improvements
* [ ] Multiple editor themes
* [ ] Improved error presentation

## Learning

* [ ] User progress dashboard
* [ ] Problem recommendations
* [ ] Daily challenge
* [ ] Streak system
* [ ] Achievements

## AI

* [ ] Improved AI tutor
* [ ] Hint system
* [ ] Code review
* [ ] Complexity analysis
* [ ] Learning recommendations

## Desktop

* [ ] Automatic updates
* [ ] GitHub Releases integration
* [ ] Improved offline handling
* [ ] Crash reporting
* [ ] Application telemetry with user privacy controls

---

# 🔗 Related Components

AlgoForge consists of several major components:

```text id="b9w6j3"
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
```

---

# 👨‍💻 Author

**Ufuk Çöz**

Software Engineering Student & Developer

GitHub:

https://github.com/ufukcoz

---

# 📄 License

The project's licensing model will be defined before official open-source distribution.

---

## Status

AlgoForge Desktop is under active development.

The application is evolving toward a production-ready desktop coding and algorithm learning environment with stronger authentication, security, testing, offline handling and automated releases.
