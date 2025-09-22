using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using NotesBackend.Application.Auth;

namespace NotesBackend.Security
{
    /// <summary>
    /// Extension methods to configure JWT auth.
    /// </summary>
    public static class JwtAuthExtensions
    {
        // PUBLIC_INTERFACE
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, JwtSettings settings)
        {
            var key = Encoding.UTF8.GetBytes(settings.Secret);
            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = settings.Issuer,
                        ValidateAudience = true,
                        ValidAudience = settings.Audience,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.FromMinutes(1)
                    };
                });

            return services;
        }
    }
}
