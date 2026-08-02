using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using WealthOS.Application.Dashboard;
using WealthOS.Application.Loans;
using WealthOS.Application.Properties;

namespace WealthOS.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services.AddValidatorsFromAssembly(assembly, includeInternalTypes: true);
        services.AddAutoMapper(assembly);
        services.AddDashboardApplication();
        services.AddPropertiesApplication();
        services.AddLoansApplication();

        return services;
    }
}
