# AlgoForge Desktop

Desktop application of the AlgoForge algorithm learning platform.

The application communicates with the AlgoForge backend through REST APIs and does not connect directly to PostgreSQL.

## Features

* Algorithm questions
* Problem browsing
* Monaco code editor
* Code submissions
* Contest features
* Leaderboard
* User profile
* AI assistant
* Backend API integration

## Technologies

* Electron
* React
* TypeScript
* Vite
* Monaco Editor
* Electron Builder

## Architecture

```text
AlgoForge Desktop
        │
        ├── Electron
        │
        └── React + TypeScript
                │
                ▼
          AlgoForge API
                │
                ▼
       ASP.NET Core Backend
```

The desktop application communicates with the backend through REST APIs.

The desktop application does not connect directly to PostgreSQL.

## Project Structure

```text
desktop/
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
```

## Installation

From the repository root:

```bash
cd desktop
npm install
```

## Development

Run the desktop development environment:

```bash
npm run electron:dev
```

## Build

Build the desktop application:

```bash
npm run electron:build
```

The packaged application is generated according to the Electron Builder configuration.

## Backend

The desktop application communicates with the AlgoForge ASP.NET Core API.

```text
AlgoForge Desktop
        │
        ▼
      REST API
        │
        ▼
ASP.NET Core Backend
```

The production API URL is configured through the desktop application's API configuration rather than through a direct database connection.

## CI

The desktop project is built through GitHub Actions.

The CI workflow performs:

* npm dependency installation
* TypeScript/Vite build
* Electron Builder packaging
* Windows installer artifact upload
