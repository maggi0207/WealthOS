using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WealthOS.Application.Authentication;
using WealthOS.Domain.Authentication.Repositories;
using WealthOS.Domain.Common.Abstractions.Repositories;
using WealthOS.Application.Common.Interfaces;
using WealthOS.Application.Authentication.Interfaces;
using WealthOS.Infrastructure.Authentication.Services;
using WealthOS.Infrastructure.Identity;
using WealthOS.Infrastructure.Persistence;
using WealthOS.Infrastructure.Persistence.Interceptors;
using WealthOS.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace WealthOS.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<AuditableEntityInterceptor>();

        services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");

            options.UseNpgsql(connectionString, npgsql =>
            {
                npgsql.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                npgsql.EnableRetryOnFailure(maxRetryCount: 3);
            });

            options.AddInterceptors(serviceProvider.GetRequiredService<AuditableEntityInterceptor>());
        });

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();

        services.AddAuthenticationOptions(configuration);

        return services;
    }
}
