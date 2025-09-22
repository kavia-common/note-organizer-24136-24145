using System.ComponentModel.DataAnnotations;

namespace NotesBackend.Api.Models
{
    /// <summary>
    /// Request payloads for authentication APIs.
    /// </summary>
    public class RegisterRequest
    {
        [Required, EmailAddress, MaxLength(256)]
        public string Email { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string DisplayName { get; set; } = string.Empty;

        [Required, MinLength(6), MaxLength(200)]
        public string Password { get; set; } = string.Empty;
    }

    public class LoginRequest
    {
        [Required, EmailAddress, MaxLength(256)]
        public string Email { get; set; } = string.Empty;

        [Required, MinLength(6), MaxLength(200)]
        public string Password { get; set; } = string.Empty;
    }

    public class AuthResponse
    {
        public string Message { get; set; } = "Signed in";
        public string AccessToken { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public DateTime ExpiresAtUtc { get; set; }
    }
}
