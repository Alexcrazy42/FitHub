# Repository Guidelines

## Project Structure & Module Organization

FitHub is a demo SaaS platform for fitness operations. Product code is split between `backend/` and `frontend/`. The backend solution is `backend/FitHub.sln`; platform code lives under `backend/Platform/src/` with `Host`, `HostJobs`, `Application`, `Domain`, `Data`, and `Contracts` projects. Shared .NET libraries are in `backend/Common/`, and backend tests are in `backend/Platform/tests/`. The frontend is a Vite React app in `frontend/`, with source under `frontend/src/`, pages under `src/pages/`, components under `src/components/`, state under `src/store/`, and API wrappers under `src/api/`. Docs and screenshots live in `docs/`, `mds/`, and `images/`; `sandbox/` contains experiments, not production code.

## Build, Test, and Development Commands

- `cd backend; dotnet build FitHub.sln` builds the .NET 9 solution.
- `cd backend; dotnet test FitHub.sln` runs xUnit unit and integration tests.
- `cd backend; ./build.ps1` runs the NUKE build entry point.
- `cd frontend; npm install` restores frontend dependencies.
- `cd frontend; npm run dev` starts the Vite dev server.
- `cd frontend; npm run build` runs TypeScript and Vite production builds.
- `cd frontend; npm run lint` runs ESLint over TypeScript/React files.
- `docker compose up` starts the root local infrastructure stack.

## Coding Style & Naming Conventions

Backend projects use nullable reference types, implicit usings, central package management, and warnings as errors via `Directory.Build.props`. Keep domain invariants in `Domain`, use cases in `Application`, persistence in `Data`, and HTTP contracts in `Contracts`. Use PascalCase for C# types and methods, `I` prefixes for interfaces, and `*Tests` names for test classes.

Frontend code is TypeScript-first. Use `.tsx` for React components, PascalCase for component files, camelCase for hooks/utilities, and shared types in `src/types/`. ESLint combines recommended JavaScript, TypeScript, React Hooks, and Vite React Refresh rules.

## Testing Guidelines

Backend tests use xUnit with Moq, Shouldly, AutoFixture, coverlet, and Testcontainers for integration cases. Place tests in `UnitTests` or `IntegrationTests` beside the matching feature area, for example `Application/Messaging/ChatServiceTests.cs`. Run targeted tests with `dotnet test backend/Platform/tests/UnitTests/UnitTests.csproj`. No frontend test runner is configured; validate frontend changes with `npm run lint` and `npm run build`.

## Commit & Pull Request Guidelines

Git history was unavailable because Git safe-directory ownership checks blocked log access. Use short, imperative commit subjects such as `Add chat attachment validation` and keep unrelated backend, frontend, docs, and sandbox changes in separate commits. Pull requests should describe the change, list verification commands, link related issues or docs, and include screenshots or recordings for UI-visible changes.
