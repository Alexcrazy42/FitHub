# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

FitHub is a vertical SaaS monolith for the fitness industry. It combines ERP (company/gym resource management) and CRM (visitor management via real-time chat) functionality. It also serves as a sandbox for evaluating specific technologies.

## Tech Stack

- **Backend**: .NET 9, ASP.NET Core, EF Core 9, PostgreSQL, SignalR, Minio S3, RabbitMQ
- **Frontend**: React 19, TypeScript, Vite, Redux Toolkit, React Hook Form, Ant Design, Tailwind CSS
- **Testing**: xUnit, Moq, AutoFixture, Shouldly, TestContainers
- **Build**: Nuke build system

## Commands

### Backend (from `backend/`)

```bash
# Build and run via Nuke
./build.sh          # Linux/macOS
build.cmd           # Windows

# Direct dotnet commands
dotnet build FitHub.sln
dotnet run --project src/Host

# Run all tests
dotnet test FitHub.sln

# Run a single test project
dotnet test tests/UnitTests
dotnet test tests/IntegrationTests

# Add EF Core migration (from solution root)
dotnet ef migrations add <MigrationName> --project src/Data --startup-project src/Host
```

### Infrastructure (from repo root)

```bash
docker-compose up -d   # Start PostgreSQL (port 5432) and Minio S3 (ports 9000/9001)
```

Default credentials: PostgreSQL `postgres/postgres`, Minio `minio/minio123`

### Frontend (from `frontend/`)

```bash
npm install
npm run dev      # Dev server at http://localhost:5173
npm run build    # TypeScript check + Vite build
npm run lint     # ESLint
```

## Architecture

### Backend Layer Structure

The backend follows Clean Architecture with three main layers, each with a `ServiceRegistry.cs` for DI registration:

```
src/
  Domain/          # Aggregates, Value Objects, domain entities — no external dependencies
  Application/     # Services + repository interfaces, use cases
  Data/            # EF Core: DataContext, migrations, repository implementations
  Web/             # ASP.NET Core controllers (V1/), FluentValidation validators, SignalR hubs
  Host/            # Entry point: Program.cs, Startup.cs, appsettings
```

**DI Auto-Registration**: Both `Application` and `Data` use reflection-based auto-registration:
- Services: any `IXxxService` interface automatically wired to its implementation
- Repositories: any `IPendingRepository<,>` or `IPendingNoIdRepository<>` interface wired to its implementation in `Data`

### Domain Modules

The codebase is organized by domain module, consistent across all layers:
- **Users** — CMS admins, gym admins, trainers, visitors; identity/session management
- **Trainings** — base group trainings, group trainings, personal trainings, muscle groups, training types
- **Equipments** — gym equipment management
- **Files** — S3 presigned URL upload flow with cleanup job
- **Messaging** — SignalR chat hub (`/chatHub`), real-time notifications
- **EmailNotifications** — email notification services

### Key Patterns

- **Strongly-typed IDs**: Each entity has a dedicated `XxxId` value type (e.g., `GroupTrainingId`)
- **Read models**: Denormalized PostgreSQL views/tables for performance-sensitive queries
- **JSONB**: Used for dynamic/BDUI-style data on entities
- **Compensating jobs**: S3 uploads use a cleanup job to ensure DB and S3 stay consistent
- **JWT + Claims**: Policy-based authorization using JWT claims; same JWT used for both REST and SignalR

### Testing

**Integration tests** use `WebApplicationFactory<Startup>` + TestContainers (PostgreSQL in Docker):
- `ServerFixture` — shared fixture that seeds data, manages `TestApplication`, and wires typed HTTP clients
- `TestApplication` — spins up a real PostgreSQL container per test collection; replaces auth and external API mocks via `ConfigureTestServices`
- `CurrentUserProvider` — controls the authenticated user identity during tests

**Unit tests** cover application services, domain logic, validators, and DI configuration.

### Frontend Structure

```
src/
  api/          # Axios API clients, one file per domain
  store/        # Redux slices (Redux Toolkit)
  pages/        # Route-level components (admin, gym-admin, user, chat, auth)
  components/   # Shared UI components
  layouts/      # Page layout wrappers
  routes/       # React Router config
  context/      # React context providers
  types/        # TypeScript type definitions
```

SignalR connection is managed via `WebSocketProvider.tsx` at the app root.

### Configuration

The backend reads config from `appsettings.json` + `appsettings.Development.json`. Key config sections:
- `Database.ConnectionString` / `Database.DatabaseProvider` — EF Core connection
- `AWS.*` — Minio S3 settings (ServiceURL, AccessKey, SecretKey, BucketName)
- `Auth.*` — JWT settings (see `AuthOptions`)
- `SkipMigration` — set to `true` to skip auto-migration on startup

CORS is configured to allow `http://localhost:5173` (Vite dev server) in development.

### Warnings Policy

`TreatWarningsAsErrors` is enabled globally. Suppressed warnings are documented in `Directory.Build.props`.
