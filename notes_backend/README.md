# Notes Backend (.NET 8)

This project is a REST API for authentication and CRUD operations on notes.

How to run locally:
1) Restore packages
   - dotnet restore notes_backend.sln
     or
   - dotnet restore

2) Build
   - dotnet build -c Release

3) Run
   - dotnet run --project dotnet.csproj

Environment variables:
- JWT_SECRET: Required in production for token signing. In local dev, defaults to empty and logs a warning.
- DB_CONNECTION_STRING: Optional. If omitted, an in-memory database is used.
  - Postgres (Npgsql) example: Host=localhost;Port=5432;Database=notes;Username=postgres;Password=postgres
  - SQL Server example: Server=localhost,1433;Database=notes;User Id=sa;Password=Your_password123;

NuGet configuration:
- A NuGet.Config is included to pin nuget.org as the source. This ensures dependencies restore properly in CI/preview environments.

API docs:
- Swagger UI: /docs
- OpenAPI JSON: /openapi/v1.json

Health:
- GET /
