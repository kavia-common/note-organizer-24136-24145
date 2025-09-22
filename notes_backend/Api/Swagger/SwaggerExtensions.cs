using NSwag;
using NSwag.Generation.Processors.Security;

namespace NotesBackend.Api.Swagger
{
    /// <summary>
    /// NSwag configuration helpers to include JWT security scheme and metadata.
    /// </summary>
    public static class SwaggerExtensions
    {
        // PUBLIC_INTERFACE
        public static IServiceCollection AddOceanSwagger(this IServiceCollection services)
        {
            services.AddOpenApiDocument(config =>
            {
                config.Title = "Notes API";
                config.Version = "v1";
                config.Description = "Modern Notes API • Ocean Professional Theme";

                // NSwag security scheme for JWT bearer
                config.AddSecurity("JWT", Array.Empty<string>(), new OpenApiSecurityScheme
                {
                    Type = OpenApiSecuritySchemeType.ApiKey,
                    Name = "Authorization",
                    In = OpenApiSecurityApiKeyLocation.Header,
                    Description = "Enter: Bearer {your JWT token}"
                });

                config.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("JWT"));
                config.DocumentName = "v1";
            });

            return services;
        }
    }
}
