using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using WealthOS.Application.AI;
using WealthOS.Application.Assets;
using WealthOS.Application.Dashboard;
using WealthOS.Application.Documents;
using WealthOS.Application.Goals;
using WealthOS.Application.Income;
using WealthOS.Application.Investments;
using WealthOS.Application.Loans;
using WealthOS.Application.Notifications;
using WealthOS.Application.Properties;
using WealthOS.Application.Reports;
using WealthOS.Application.Settings;

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
        services.AddIncomeApplication();
        services.AddInvestmentsApplication();
        services.AddGoalsApplication();
        services.AddDocumentsApplication();
        services.AddAssetsApplication();
        services.AddNotificationsApplication();
        services.AddAIApplication();
        services.AddReportsApplication();
        services.AddSettingsApplication();

        return services;
    }
}
