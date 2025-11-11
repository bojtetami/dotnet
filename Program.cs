using NotesApp.Models;
using NotesApp.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(provider =>
{
	var dataDir = Path.Combine(builder.Environment.ContentRootPath, "App_Data");
	var storagePath = Path.Combine(dataDir, "notes.json");
	return new NoteStore(storagePath);
});

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapGet("/api/notes", (NoteStore store) =>
{
	return Results.Ok(store.GetAll());
});

app.MapPost("/api/notes", async (HttpContext httpContext, NoteStore store) =>
{
	var note = await httpContext.Request.ReadFromJsonAsync<Note>();
	if (note == null || string.IsNullOrWhiteSpace(note.Title))
	{
		return Results.BadRequest(new { message = "Title is required" });
	}
	note = new Note
	{
		Title = note.Title.Trim(),
		Content = note.Content?.Trim() ?? string.Empty
	};
	var added = store.Add(note);
	return Results.Created($"/api/notes/{added.Id}", added);
});

app.MapDelete("/api/notes/{id:guid}", (Guid id, NoteStore store) =>
{
	return store.Delete(id) ? Results.NoContent() : Results.NotFound();
});

app.Run();
