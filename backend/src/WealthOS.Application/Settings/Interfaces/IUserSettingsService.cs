using WealthOS.Application.Common.Models;
using WealthOS.Application.Settings.DTOs.Requests;
using WealthOS.Application.Settings.DTOs.Responses;

namespace WealthOS.Application.Settings.Interfaces;

public interface IUserSettingsService
{
    Task<Result<UserSettingsResponse>> GetAsync(CancellationToken cancellationToken = default);

    Task<Result<UserSettingsResponse>> UpdateAsync(
        UpdateSettingsRequest request,
        CancellationToken cancellationToken = default);

    Task<Result<UserSettingsResponse>> UpdateProfileAsync(
        UpdateProfileSettingsRequest request,
        CancellationToken cancellationToken = default);

    Task<Result<UserSettingsResponse>> UpdatePreferencesAsync(
        UpdatePreferencesSettingsRequest request,
        CancellationToken cancellationToken = default);

    Task<Result<UserSettingsResponse>> UpdateNotificationsAsync(
        UpdateNotificationSettingsRequest request,
        CancellationToken cancellationToken = default);

    Task<Result<UserSettingsResponse>> UpdateSecurityAsync(
        UpdateSecuritySettingsRequest request,
        CancellationToken cancellationToken = default);

    Task<Result<SettingsExportResponse>> ExportAsync(
        ExportSettingsRequest request,
        CancellationToken cancellationToken = default);

    Task<Result<UserSettingsResponse>> ImportAsync(
        ImportSettingsRequest request,
        CancellationToken cancellationToken = default);

    Task<Result> ClearCacheAsync(CancellationToken cancellationToken = default);

    Task<Result> DeleteAccountAsync(CancellationToken cancellationToken = default);
}
