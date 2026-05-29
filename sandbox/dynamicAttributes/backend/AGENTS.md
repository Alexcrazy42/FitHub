# Repository Guidelines

## Project Structure & Module Organization
This repository is a .NET 9 Web API solution. The solution file lives at the repo root as `Backend.sln`, and the main project is in `Backend/`.

- `Backend/Controllers`: HTTP endpoints such as `ProductsController`
- `Backend/Services`: domain logic such as facet calculation
- `Backend/Data`: EF Core context, repositories, and seed logic
- `Backend/Models`: persistence models
- `Backend/Dtos`: request and response contracts
- `Backend/Properties/launchSettings.json`: local run profiles

Ignore generated output under `Backend/bin` and `Backend/obj`.

## Build, Test, and Development Commands
Run commands from the repository root.

- `dotnet restore Backend.sln`: restore NuGet packages
- `dotnet build Backend.sln`: compile the solution
- `dotnet run --project Backend/Backend.csproj`: start the API locally
- `dotnet watch run --project Backend/Backend.csproj`: run with hot reload during development

The app uses PostgreSQL through EF Core and applies migrations plus seed data on startup. Default local URLs are defined in `Backend/Properties/launchSettings.json` (`http://localhost:5118`, `https://localhost:7209`).

## Coding Style & Naming Conventions
Use standard C# conventions with 4-space indentation and nullable reference types enabled.

- Types, methods, and public properties: `PascalCase`
- Local variables and parameters: `camelCase`
- DTO classes: suffix with `Dto` or `Dtos` to match current project usage
- Controllers: suffix with `Controller`

Keep controllers thin, move business logic into `Services`, and keep EF Core access in `Data`.

## Testing Guidelines
There is no dedicated test project yet. When adding tests, create a sibling project such as `Backend.Tests` and include it in `Backend.sln`.

- Prefer xUnit for API and service tests
- Name files after the unit under test, for example `FacetServiceTests.cs`
- Run tests with `dotnet test Backend.sln`

Focus coverage on filtering logic, repository queries, and controller response contracts.

## Commit & Pull Request Guidelines
Git history is not accessible from this sandbox snapshot, so no repository-specific commit pattern could be verified. Use short, imperative commit messages such as `Add facet filtering for category search`.

For pull requests, include:

- a concise summary of behavior changes
- linked issue or task ID when available
- test evidence (`dotnet build`, `dotnet test`)
- sample request/response payloads for API changes

## Configuration & Security Notes
Store environment-specific settings in `appsettings.Development.json` or user secrets, not hardcoded credentials. Review CORS origins in `Program.cs` when adding new frontend clients.
