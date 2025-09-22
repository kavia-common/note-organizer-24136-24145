using System.Security.Cryptography;
using System.Text;

namespace NotesBackend.Application.Auth
{
    /// <summary>
    /// Provides password hashing and verification using HMACSHA512.
    /// </summary>
    public static class PasswordHasher
    {
        public static (byte[] hash, byte[] salt) HashPassword(string password)
        {
            using var hmac = new HMACSHA512();
            var salt = hmac.Key;
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return (hash, salt);
        }

        public static bool VerifyPassword(string password, byte[] hash, byte[] salt)
        {
            using var hmac = new HMACSHA512(salt);
            var computed = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return computed.SequenceEqual(hash);
        }
    }
}
