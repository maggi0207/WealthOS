using WealthOS.Application.Common.Abstractions;
using WealthOS.Application.Settings.DTOs.Requests;

namespace WealthOS.Application.Settings.Commands;

public sealed class UpdateUserSettingsCommand : ICommand
{
    public required UpdateSettingsRequest Request { get; init; }
}

public sealed class UpdateProfileSettingsCommand : ICommand
{
    public required UpdateProfileSettingsRequest Request { get; init; }
}

public sealed class UpdatePreferencesSettingsCommand : ICommand
{
    public required UpdatePreferencesSettingsRequest Request { get; init; }
}

public sealed class UpdateNotificationSettingsCommand : ICommand
{
    public required UpdateNotificationSettingsRequest Request { get; init; }
}

public sealed class UpdateSecuritySettingsCommand : ICommand
{
    public required UpdateSecuritySettingsRequest Request { get; init; }
}

public sealed class ExportSettingsCommand : ICommand
{
    public required ExportSettingsRequest Request { get; init; }
}

public sealed class ImportSettingsCommand : ICommand
{
    public required ImportSettingsRequest Request { get; init; }
}

public sealed class ClearSettingsCacheCommand : ICommand
{
}

public sealed class DeleteAccountCommand : ICommand
{
}
