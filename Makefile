SOLUTION := note-organizer.sln

.PHONY: restore build clean run

restore:
\tdotnet restore $(SOLUTION)

build: restore
\tdotnet build $(SOLUTION) -c Release

clean:
\tdotnet clean $(SOLUTION)

run:
\tdotnet run --project notes_backend/dotnet.csproj
