using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WealthOS.Application.Authentication.Options;
using WealthOS.Domain.Authentication.Constants;
using WealthOS.Domain.Authentication.Entities;

namespace WealthOS.Infrastructure.Identity;

public static class IdentityDataSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        using var scope = serviceProvider.CreateScope();
        var services = scope.ServiceProvider;

        var logger = services.GetRequiredService<ILoggerFactory>().CreateLogger("IdentityDataSeeder");
        var roleManager = services.GetRequiredService<RoleManager<Role>>();
        var userManager = services.GetRequiredService<UserManager<User>>();
        var adminSeed = services.GetRequiredService<IOptions<AdminSeedSettings>>().Value;

        await EnsureRoleAsync(roleManager, AuthRoles.Admin, "Full administrative access", logger);
        await EnsureRoleAsync(roleManager, AuthRoles.User, "Standard authenticated user", logger);

        if (string.IsNullOrWhiteSpace(adminSeed.Password))
        {
            logger.LogWarning(
                "Admin seed password is not configured. Skipping admin user seed. Set AdminSeed:Password via configuration or environment.");
            return;
        }

        var existingAdmin = await userManager.FindByEmailAsync(adminSeed.Email);
        if (existingAdmin is not null)
        {
            if (!await userManager.IsInRoleAsync(existingAdmin, AuthRoles.Admin))
            {
                await userManager.AddToRoleAsync(existingAdmin, AuthRoles.Admin);
            }

            logger.LogInformation("Admin user already exists for {Email}", adminSeed.Email);
            return;
        }

        var adminUser = new User
        {
            Id = Guid.NewGuid(),
            UserName = adminSeed.Email,
            Email = adminSeed.Email,
            EmailConfirmed = true,
            FirstName = adminSeed.FirstName,
            LastName = adminSeed.LastName,
            DisplayName = $"{adminSeed.FirstName} {adminSeed.LastName}",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
        };

        var createResult = await userManager.CreateAsync(adminUser, adminSeed.Password);
        if (!createResult.Succeeded)
        {
            var errors = string.Join(", ", createResult.Errors.Select(error => error.Description));
            logger.LogError("Failed to seed admin user: {Errors}", errors);
            return;
        }

        await userManager.AddToRoleAsync(adminUser, AuthRoles.Admin);
        logger.LogInformation("Seeded admin user {Email}", adminSeed.Email);
    }

    private static async Task EnsureRoleAsync(
        RoleManager<Role> roleManager,
        string roleName,
        string description,
        ILogger logger)
    {
        if (await roleManager.RoleExistsAsync(roleName))
        {
            return;
        }

        var role = new Role
        {
            Id = Guid.NewGuid(),
            Name = roleName,
            NormalizedName = roleName.ToUpperInvariant(),
            Description = description,
            CreatedAt = DateTime.UtcNow,
        };

        var result = await roleManager.CreateAsync(role);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(error => error.Description));
            logger.LogError("Failed to seed role {RoleName}: {Errors}", roleName, errors);
            return;
        }

        logger.LogInformation("Seeded role {RoleName}", roleName);
    }
}
