using Microsoft.Extensions.DependencyInjection;
using WealthOS.Application.Common.Abstractions;
using WealthOS.Application.Settings.Commands;
using WealthOS.Application.Settings.Commands.Handlers;
using WealthOS.Application.Settings.DTOs.Responses;
using WealthOS.Application.Settings.Interfaces;
using WealthOS.Application.Settings.Queries;
using WealthOS.Application.Settings.Services;

namespace WealthOS.Application.Settings;

public static class SettingsServiceCollectionExtensions
{
    public static IServiceCollection AddSettingsApplication(this IServiceCollection services)
    {
        services.AddScoped<IUserSettingsService, UserSettingsService>();

        services.AddScoped<IQueryHandler<GetUserSettingsQuery, UserSettingsResponse>, GetUserSettingsQueryHandler>();
        services.AddScoped<ICommandHandler<UpdateUserSettingsCommand, UserSettingsResponse>, UpdateUserSettingsCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateProfileSettingsCommand, UserSettingsResponse>, UpdateProfileSettingsCommandHandler>();
        services.AddScoped<ICommandHandler<UpdatePreferencesSettingsCommand, UserSettingsResponse>, UpdatePreferencesSettingsCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateNotificationSettingsCommand, UserSettingsResponse>, UpdateNotificationSettingsCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateSecuritySettingsCommand, UserSettingsResponse>, UpdateSecuritySettingsCommandHandler>();
        services.AddScoped<ICommandHandler<ExportSettingsCommand, SettingsExportResponse>, ExportSettingsCommandHandler>();
        services.AddScoped<ICommandHandler<ImportSettingsCommand, UserSettingsResponse>, ImportSettingsCommandHandler>();
        services.AddScoped<ICommandHandler<ClearSettingsCacheCommand>, ClearSettingsCacheCommandHandler>();
        services.AddScoped<ICommandHandler<DeleteAccountCommand>, DeleteAccountCommandHandler>();

        return services;
    }
}
