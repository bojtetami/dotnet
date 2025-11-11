using System.Collections.Concurrent;
using System.Text.Json;
using NotesApp.Models;

namespace NotesApp.Services;

public class NoteStore
{
	private readonly string _storageFilePath;
	private readonly ConcurrentDictionary<Guid, Note> _notes = new();
	private readonly JsonSerializerOptions _jsonOptions = new()
	{
		WriteIndented = true
	};

	public NoteStore(string storageFilePath)
	{
		_storageFilePath = storageFilePath;
		Directory.CreateDirectory(Path.GetDirectoryName(_storageFilePath) ?? ".");
		LoadFromDisk();
	}

	public IReadOnlyCollection<Note> GetAll() => _notes.Values.OrderByDescending(n => n.CreatedAt).ToList();

	public Note Add(Note note)
	{
		if (note.Id == Guid.Empty) note.Id = Guid.NewGuid();
		if (note.CreatedAt == default) note.CreatedAt = DateTimeOffset.UtcNow;
		_notes[note.Id] = note;
		SaveToDisk();
		return note;
	}

	public bool Delete(Guid id)
	{
		var removed = _notes.TryRemove(id, out _);
		if (removed) SaveToDisk();
		return removed;
	}

	private void LoadFromDisk()
	{
		if (!File.Exists(_storageFilePath))
		{
			return;
		}
		try
		{
			var json = File.ReadAllText(_storageFilePath);
			var list = JsonSerializer.Deserialize<List<Note>>(json) ?? new List<Note>();
			foreach (var note in list)
			{
				_notes[note.Id] = note;
			}
		}
		catch
		{
			// ignore malformed storage file
		}
	}

	private void SaveToDisk()
	{
		var list = _notes.Values.OrderByDescending(n => n.CreatedAt).ToList();
		var json = JsonSerializer.Serialize(list, _jsonOptions);
		File.WriteAllText(_storageFilePath, json);
	}
}


