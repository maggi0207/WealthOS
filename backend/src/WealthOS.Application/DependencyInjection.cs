using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using WealthOS.Application.Dashboard;

namespace WealthOS.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services.AddValidatorsFromAssembly(assembly, includeInternalTypes: true);
        services.AddAutoMapper(assembly);
        services.AddDashboardApplication();

        return services;
    }
}
