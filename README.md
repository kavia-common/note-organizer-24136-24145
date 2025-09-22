# Note Organizer

Containers:
- notes_backend (dotnet 8)

Build/Run notes_backend:
- From repo root (preferred for CI):
  - dotnet restore note-organizer.sln
  - dotnet build note-organizer.sln -c Release
  - dotnet run --project notes_backend/dotnet.csproj

Alternative (from container root):
  - dotnet restore notes_backend.sln
  - dotnet build notes_backend.sln -c Release
  - dotnet run --project dotnet.csproj

Note:
- A Directory.Build.rsp and msbuild.rsp are provided so bare msbuild/dotnet build from repo root will target the root solution.
- A Default.Build.proj proxy exists; if your environment insists on invoking `msbuild` or `dotnet build` without args, run:
  - dotnet build Default.Build.proj
- If invoking `dotnet build` without specifying a project/solution, MSBuild may error when multiple project/solution files are present. Prefer the explicit commands above.
