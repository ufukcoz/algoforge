
---

## 3. `desktop/README.md`

```markdown
# AlgoForge Desktop

Desktop application of the AlgoForge algorithm learning platform.

The application allows users to browse problems, write code, submit solutions and use the platform's learning features.

## ✨ Features

- Algorithm questions
- Monaco code editor
- Code submissions
- Contest features
- Leaderboard
- User profile
- AI assistant
- Backend API integration

## 🛠️ Technologies

- Electron
- React
- TypeScript
- Vite
- Monaco Editor
- Electron Builder

## 🏗️ Architecture

```text
AlgoForge Desktop
        │
        ├── Electron
        │
        └── React + TypeScript
                │
                ▼
          AlgoForge API

The desktop application communicates with the backend through REST APIs.

The desktop application does not connect directly to PostgreSQL.

📂 Structure
desktop/
│
├── src/
│   ├── components/
│   ├── pages/
│   ├── contexts/
│   └── ...
│
├── electron/
├── public/
├── package.json
└── README.md
🚀 Installation

From the repository root:

cd desktop
npm install
🧑‍💻 Development

Run the development environment:

npm run electron:dev
📦 Build

Build the application:

npm run electron:build

The packaged application is generated according to the Electron Builder configuration.

🌐 Backend

The desktop application communicates with the AlgoForge ASP.NET Core API.

AlgoForge Desktop
        │
        ▼
   REST API
        │
        ▼
ASP.NET Core Backend
