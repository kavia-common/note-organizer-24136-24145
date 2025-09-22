using System.ComponentModel.DataAnnotations;

namespace NotesBackend.Domain.Entities
{
    /// <summary>
    /// User entity represents an application user capable of owning Notes.
    /// </summary>
    public class User
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required, MaxLength(256)]
        public string Email { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string DisplayName { get; set; } = string.Empty;

        [Required]
        public byte[] PasswordHash { get; set; } = Array.Empty<byte>();

        [Required]
        public byte[] PasswordSalt { get; set; } = Array.Empty<byte>();

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        public ICollection<Note> Notes { get; set; } = new List<Note>();
    }
}
