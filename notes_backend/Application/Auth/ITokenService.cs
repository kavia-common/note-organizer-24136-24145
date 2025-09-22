using NotesBackend.Domain.Entities;

namespace NotesBackend.Application.Auth
{
    /// <summary>
    /// Contract for producing JWT access tokens.
    /// </summary>
    public interface ITokenService
    {
        string CreateToken(User user);
    }
}
