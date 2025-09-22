using Microsoft.Extensions.Logging;
using NotesBackend.Domain.Entities;
using NotesBackend.Infrastructure.Repositories;

namespace NotesBackend.Application.Notes
{
    /// <summary>
    /// Note service implements validation and repository orchestration.
    /// </summary>
    public class NoteService : INoteService
    {
        private readonly INoteRepository _notes;
        private readonly ILogger<NoteService> _logger;

        public NoteService(INoteRepository notes, ILogger<NoteService> logger)
        {
            _notes = notes;
            _logger = logger;
        }

        public async Task<Note> CreateAsync(Guid ownerId, string title, string content, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Title is required.", nameof(title));
            if (string.IsNullOrWhiteSpace(content)) throw new ArgumentException("Content is required.", nameof(content));

            var note = new Note
            {
                UserId = ownerId,
                Title = title.Trim(),
                Content = content.Trim()
            };
            await _notes.AddAsync(note, ct);
            await _notes.SaveChangesAsync(ct);

            _logger.LogInformation("Ocean: Note created • NoteId={NoteId} • Owner={OwnerId}", note.Id, ownerId);
            return note;
        }

        public Task<Note?> GetAsync(Guid ownerId, Guid noteId, CancellationToken ct = default)
        {
            return _notes.GetByIdAsync(noteId, ownerId, ct);
        }

        public async Task<(IEnumerable<Note> items, int total)> ListAsync(Guid ownerId, int page, int pageSize, CancellationToken ct = default)
        {
            page = page <= 0 ? 1 : page;
            pageSize = pageSize <= 0 || pageSize > 100 ? 20 : pageSize;

            var items = await _notes.ListAsync(ownerId, page, pageSize, ct);
            var count = await _notes.CountAsync(ownerId, ct);
            return (items, count);
        }

        public async Task<Note?> UpdateAsync(Guid ownerId, Guid noteId, string title, string content, CancellationToken ct = default)
        {
            var note = await _notes.GetByIdAsync(noteId, ownerId, ct);
            if (note == null) return null;

            if (!string.IsNullOrWhiteSpace(title)) note.Title = title.Trim();
            if (!string.IsNullOrWhiteSpace(content)) note.Content = content.Trim();
            note.UpdatedAtUtc = DateTime.UtcNow;

            _notes.Update(note);
            await _notes.SaveChangesAsync(ct);

            _logger.LogInformation("Ocean: Note updated • NoteId={NoteId} • Owner={OwnerId}", note.Id, ownerId);
            return note;
        }

        public async Task<bool> DeleteAsync(Guid ownerId, Guid noteId, CancellationToken ct = default)
        {
            var note = await _notes.GetByIdAsync(noteId, ownerId, ct);
            if (note == null) return false;

            _notes.Remove(note);
            await _notes.SaveChangesAsync(ct);

            _logger.LogInformation("Ocean: Note deleted • NoteId={NoteId} • Owner={OwnerId}", note.Id, ownerId);
            return true;
        }
    }
}
