using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotesBackend.Api.Models;
using NotesBackend.Application.Auth;
using NotesBackend.Application.Users;

namespace NotesBackend.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;
        private readonly JwtSettings _jwtSettings;

        public AuthController(IUserService userService, ITokenService tokenService, JwtSettings jwtSettings)
        {
            _userService = userService;
            _tokenService = tokenService;
            _jwtSettings = jwtSettings;
        }

        /// <summary>
        /// Registers a new user.
        /// </summary>
        [HttpPost("register")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterRequest req, CancellationToken ct)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            try
            {
                var user = await _userService.RegisterAsync(req.Email, req.DisplayName, req.Password, ct);
                var token = _tokenService.CreateToken(user);
                var expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes);

                var resp = new AuthResponse
                {
                    Message = "Ocean: Welcome aboard! Your account is ready.",
                    AccessToken = token,
                    UserId = user.Id,
                    Email = user.Email,
                    DisplayName = user.DisplayName,
                    ExpiresAtUtc = expires
                };
                return CreatedAtAction(nameof(Register), resp);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = $"Ocean: {ex.Message}" });
            }
        }

        /// <summary>
        /// Authenticates a user and returns a JWT access token.
        /// </summary>
        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginRequest req, CancellationToken ct)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var user = await _userService.ValidateCredentialsAsync(req.Email, req.Password, ct);
            if (user == null)
            {
                return Unauthorized(new { message = "Ocean: Invalid credentials. Please retry." });
            }

            var token = _tokenService.CreateToken(user);
            var expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes);

            return Ok(new AuthResponse
            {
                Message = "Ocean: Signed in successfully.",
                AccessToken = token,
                UserId = user.Id,
                Email = user.Email,
                DisplayName = user.DisplayName,
                ExpiresAtUtc = expires
            });
        }
    }
}
