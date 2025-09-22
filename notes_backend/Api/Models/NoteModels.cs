using System.ComponentModel.DataAnnotations;

namespace NotesBackend.Api.Models
{
    /// <summary>
    /// DTOs for Notes endpoints.
    /// </summary>
    public class NoteCreateRequest
    {
        [Required, MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;
    }

    public class NoteUpdateRequest
    {
        [MaxLength(200)]
        public string? Title { get; set; }

        public string? Content { get; set; }
    }

    public class NoteResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAtUtc { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }
    }

    public class PagedNotesResponse
    {
        public IEnumerable<NoteResponse> Items { get; set; } = Enumerable.Empty<NoteResponse>();
        public int Total { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
