using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WealthOS.Application.Authentication.Options;

namespace WealthOS.Application.Authentication;

public static class AuthenticationServiceCollectionExtensions
{
    public static IServiceCollection AddAuthenticationOptions(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddOptions<JwtSettings>()
            .Bind(configuration.GetSection(JwtSettings.SectionName))
            .Validate(
                settings =>
                    !string.IsNullOrWhiteSpace(settings.Issuer)
                    && !string.IsNullOrWhiteSpace(settings.Audience)
                    && !string.IsNullOrWhiteSpace(settings.SecretKey)
                    && settings.AccessTokenExpirationMinutes > 0
                    && settings.RefreshTokenExpirationDays > 0,
                "JWT settings are invalid or incomplete.")
            .ValidateOnStart();

        return services;
    }
}
