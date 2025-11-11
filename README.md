# NotesApp

A very simple note taking app built with ASP.NET Core minimal APIs and a static HTML frontend.

## Running locally

1. Prerequisites: .NET 9 SDK
2. Run the app:
   ```bash
   cd NotesApp
   dotnet run
   ```
3. Open `http://localhost:5000` (or the URL shown in the console).

Notes are stored in `NotesApp/App_Data/notes.json` (gitignored).

## API

- `GET /api/notes` — list notes
- `POST /api/notes` — create note, JSON body: `{ \"title\": \"...\", \"content\": \"...\" }`
- `DELETE /api/notes/{id}` — delete note by id

## Security scanning (Snyk)

You can run Snyk scans if you have the Snyk CLI and token:

```bash
export SNYK_TOKEN=YOUR_TOKEN
snyk code test
snyk test
```


