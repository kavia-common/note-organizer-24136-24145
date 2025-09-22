namespace NotesBackend.Application.Auth
{
    /// <summary>
    /// Strongly-typed JWT settings.
    /// </summary>
    public class JwtSettings
    {
        public const string SectionName = "Jwt";
        public string Issuer { get; set; } = "notes_backend";
        public string Audience { get; set; } = "notes_clients";
        public string Secret { get; set; } = ""; // Must be provided via environment
        public int ExpiryMinutes { get; set; } = 60;
    }
}
