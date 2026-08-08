using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using WealthOS.Application.Common.Interfaces;
using WealthOS.Application.Common.Models;
using WealthOS.Application.Settings.DTOs.Requests;
using WealthOS.Application.Settings.DTOs.Responses;
using WealthOS.Application.Settings.Interfaces;
using WealthOS.Domain.Authentication.Entities;
using WealthOS.Domain.Authentication.Repositories;
using WealthOS.Domain.Common.Abstractions.Repositories;
using WealthOS.Domain.Settings.Entities;
using WealthOS.Domain.Settings.Repositories;

namespace WealthOS.Application.Settings.Services;

public sealed class UserSettingsService : IUserSettingsService
{
    private static readonly HashSet<string> AllowedThemes = ["system", "light", "dark"];
    private static readonly HashSet<string> AllowedDensities = ["comfortable", "compact"];
    private static readonly HashSet<string> AllowedCurrencies = ["INR", "USD", "EUR", "GBP"];
    private static readonly HashSet<string> AllowedLocales = ["en-IN", "en-US", "en-GB"];
    private static readonly HashSet<string> AllowedDateFormats = ["DD/MM/YYYY", "MM/DD/YYYY", "YYYY-MM-DD"];
    private static readonly HashSet<string> AllowedNumberFormats = ["indian", "international"];

    private readonly IUserSettingsRepository _settingsRepository;
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly UserManager<User> _userManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public UserSettingsService(
        IUserSettingsRepository settingsRepository,
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        UserManager<User> userManager,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser)
    {
        _settingsRepository = settingsRepository;
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _userManager = userManager;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<Result<UserSettingsResponse>> GetAsync(CancellationToken cancellationToken = default)
    {
        var context = await LoadContextAsync(cancellationToken);
        if (context.IsFailure)
        {
            return Result.Failure<UserSettingsResponse>(context.Error!);
        }

        var tokens = await _refreshTokenRepository.GetActiveTokensByUserIdAsync(
            context.Value.User.Id,
            cancellationToken);

        return Result.Success(Map(context.Value.User, context.Value.Settings, tokens));
    }

    public async Task<Result<UserSettingsResponse>> UpdateAsync(
        UpdateSettingsRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.Profile is not null)
        {
            var profile = await UpdateProfileAsync(request.Profile, cancellationToken);
            if (profile.IsFailure)
            {
                return profile;
            }
        }

        if (request.Preferences is not null)
        {
            var prefs = await UpdatePreferencesAsync(request.Preferences, cancellationToken);
            if (prefs.IsFailure)
            {
                return prefs;
            }
        }

        if (request.Notifications is not null)
        {
            var notes = await UpdateNotificationsAsync(request.Notifications, cancellationToken);
            if (notes.IsFailure)
            {
                return notes;
            }
        }

        if (!string.IsNullOrWhiteSpace(request.AngelOneAction))
        {
            var context = await LoadContextAsync(cancellationToken);
            if (context.IsFailure)
            {
                return Result.Failure<UserSettingsResponse>(context.Error!);
            }

            context.Value.Settings.AngelOneConnected =
                request.AngelOneAction.Equals("connect", StringComparison.OrdinalIgnoreCase);
            _settingsRepository.Update(context.Value.Settings);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return await GetAsync(cancellationToken);
    }

    public async Task<Result<UserSettingsResponse>> UpdateProfileAsync(
        UpdateProfileSettingsRequest request,
        CancellationToken cancellationToken = default)
    {
        var context = await LoadContextAsync(cancellationToken);
        if (context.IsFailure)
        {
            return Result.Failure<UserSettingsResponse>(context.Error!);
        }

        var firstName = request.FirstName?.Trim() ?? string.Empty;
        var lastName = request.LastName?.Trim() ?? string.Empty;
        var workspace = request.WorkspaceName?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
        {
            return Result.Failure<UserSettingsResponse>(
                Error.Validation(
                    "Name is required.",
                    new Dictionary<string, string[]>
                    {
                        ["firstName"] = ["First name is required."],
                        ["lastName"] = ["Last name is required."],
                    }));
        }

        if (string.IsNullOrWhiteSpace(workspace))
        {
            return Result.Failure<UserSettingsResponse>(
                Error.Validation(
                    "Workspace name is required.",
                    new Dictionary<string, string[]> { ["workspaceName"] = ["Workspace name is required."] }));
        }

        var user = context.Value.User;
        user.FirstName = firstName;
        user.LastName = lastName;
        user.DisplayName = $"{firstName} {lastName}".Trim();
        _userRepository.Update(user);

        var settings = context.Value.Settings;
        settings.WorkspaceName = workspace;
        settings.AvatarUrl = string.IsNullOrWhiteSpace(request.AvatarUrl) ? null : request.AvatarUrl.Trim();
        settings.Timezone = string.IsNullOrWhiteSpace(request.Timezone) ? "Asia/Kolkata" : request.Timezone.Trim();
        settings.Country = string.IsNullOrWhiteSpace(request.Country) ? "IN" : request.Country.Trim().ToUpperInvariant();
        _settingsRepository.Update(settings);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return await GetAsync(cancellationToken);
    }

    public async Task<Result<UserSettingsResponse>> UpdatePreferencesAsync(
        UpdatePreferencesSettingsRequest request,
        CancellationToken cancellationToken = default)
    {
        var context = await LoadContextAsync(cancellationToken);
        if (context.IsFailure)
        {
            return Result.Failure<UserSettingsResponse>(context.Error!);
        }

        var theme = Normalize(request.Theme, "dark");
        var density = Normalize(request.LayoutDensity, "comfortable");
        var currency = Normalize(request.CurrencyCode, "INR").ToUpperInvariant();
        var locale = Normalize(request.Locale, "en-IN");
        var dateFormat = Normalize(request.DateFormat, "DD/MM/YYYY");
        var numberFormat = Normalize(request.NumberFormat, "indian").ToLowerInvariant();

        if (!AllowedThemes.Contains(theme)
            || !AllowedDensities.Contains(density)
            || !AllowedCurrencies.Contains(currency)
            || !AllowedLocales.Contains(locale)
            || !AllowedDateFormats.Contains(dateFormat)
            || !AllowedNumberFormats.Contains(numberFormat))
        {
            return Result.Failure<UserSettingsResponse>(
                Error.Validation(
                    "One or more preference values are invalid.",
                    new Dictionary<string, string[]> { ["preferences"] = ["Invalid preference values."] }));
        }

        var settings = context.Value.Settings;
        settings.Theme = theme;
        settings.LayoutDensity = density;
        settings.SidebarCollapsed = request.SidebarCollapsed;
        settings.CurrencyCode = currency;
        settings.Locale = locale;
        settings.DateFormat = dateFormat;
        settings.NumberFormat = numberFormat;
        _settingsRepository.Update(settings);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return await GetAsync(cancellationToken);
    }

    public async Task<Result<UserSettingsResponse>> UpdateNotificationsAsync(
        UpdateNotificationSettingsRequest request,
        CancellationToken cancellationToken = default)
    {
        var context = await LoadContextAsync(cancellationToken);
        if (context.IsFailure)
        {
            return Result.Failure<UserSettingsResponse>(context.Error!);
        }

        var settings = context.Value.Settings;
        settings.EmailNotifications = request.EmailNotifications;
        settings.PushNotifications = request.PushNotifications;
        settings.GoalReminders = request.GoalReminders;
        settings.LoanEmiReminders = request.LoanEmiReminders;
        settings.InvestmentAlerts = request.InvestmentAlerts;
        settings.AiAdvisorInsights = request.AiAdvisorInsights;
        settings.WeeklySummary = request.WeeklySummary;
        settings.MonthlyReport = request.MonthlyReport;
        _settingsRepository.Update(settings);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return await GetAsync(cancellationToken);
    }

    public async Task<Result<UserSettingsResponse>> UpdateSecurityAsync(
        UpdateSecuritySettingsRequest request,
        CancellationToken cancellationToken = default)
    {
        var context = await LoadContextAsync(cancellationToken);
        if (context.IsFailure)
        {
            return Result.Failure<UserSettingsResponse>(context.Error!);
        }

        var user = context.Value.User;
        var settings = context.Value.Settings;

        if (!string.IsNullOrWhiteSpace(request.NewPassword))
        {
            if (string.IsNullOrWhiteSpace(request.CurrentPassword))
            {
                return Result.Failure<UserSettingsResponse>(
                    Error.Validation(
                        "Current password is required.",
                        new Dictionary<string, string[]> { ["currentPassword"] = ["Current password is required."] }));
            }

            if (request.NewPassword != request.ConfirmPassword)
            {
                return Result.Failure<UserSettingsResponse>(
                    Error.Validation(
                        "Passwords do not match.",
                        new Dictionary<string, string[]> { ["confirmPassword"] = ["Passwords do not match."] }));
            }

            var change = await _userManager.ChangePasswordAsync(
                user,
                request.CurrentPassword,
                request.NewPassword);

            if (!change.Succeeded)
            {
                var message = string.Join(" ", change.Errors.Select(error => error.Description));
                return Result.Failure<UserSettingsResponse>(Error.Failure("password_change_failed", message));
            }
        }

        if (request.TwoFactorEnabled.HasValue)
        {
            settings.TwoFactorEnabled = request.TwoFactorEnabled.Value;
            _settingsRepository.Update(settings);
        }

        if (request.SignOutAllDevices == true)
        {
            var tokens = await _refreshTokenRepository.GetActiveTokensByUserIdAsync(user.Id, cancellationToken);
            foreach (var token in tokens)
            {
                token.RevokedAt = DateTime.UtcNow;
                token.RevokedByIp = "settings-signout-all";
                _refreshTokenRepository.Update(token);
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return await GetAsync(cancellationToken);
    }

    public async Task<Result<SettingsExportResponse>> ExportAsync(
        ExportSettingsRequest request,
        CancellationToken cancellationToken = default)
    {
        var settingsResult = await GetAsync(cancellationToken);
        if (settingsResult.IsFailure)
        {
            return Result.Failure<SettingsExportResponse>(settingsResult.Error!);
        }

        var scope = string.IsNullOrWhiteSpace(request.Scope) ? "all" : request.Scope.Trim().ToLowerInvariant();
        var payload = new
        {
            exportedAt = DateTime.UtcNow,
            scope,
            settings = settingsResult.Value,
        };

        var json = JsonSerializer.Serialize(payload, new JsonSerializerOptions { WriteIndented = true });
        var bytes = Encoding.UTF8.GetBytes(json);

        return Result.Success(new SettingsExportResponse
        {
            FileName = $"wealthos-{scope}-{DateTime.UtcNow:yyyyMMddHHmmss}.json",
            ContentType = "application/json",
            ContentBase64 = Convert.ToBase64String(bytes),
            GeneratedAt = DateTime.UtcNow,
        });
    }

    public async Task<Result<UserSettingsResponse>> ImportAsync(
        ImportSettingsRequest request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.ContentBase64))
        {
            return Result.Failure<UserSettingsResponse>(
                Error.Validation(
                    "Import payload is required.",
                    new Dictionary<string, string[]> { ["contentBase64"] = ["File content is required."] }));
        }

        try
        {
            var bytes = Convert.FromBase64String(request.ContentBase64);
            var json = Encoding.UTF8.GetString(bytes);
            using var document = JsonDocument.Parse(json);

            if (document.RootElement.TryGetProperty("settings", out var settingsNode))
            {
                var imported = JsonSerializer.Deserialize<UserSettingsResponse>(settingsNode.GetRawText());
                if (imported is not null)
                {
                    await UpdatePreferencesAsync(
                        new UpdatePreferencesSettingsRequest
                        {
                            Theme = imported.Theme,
                            LayoutDensity = imported.LayoutDensity,
                            SidebarCollapsed = imported.SidebarCollapsed,
                            CurrencyCode = imported.CurrencyCode,
                            Locale = imported.Locale,
                            DateFormat = imported.DateFormat,
                            NumberFormat = imported.NumberFormat,
                        },
                        cancellationToken);

                    await UpdateNotificationsAsync(
                        new UpdateNotificationSettingsRequest
                        {
                            EmailNotifications = imported.EmailNotifications,
                            PushNotifications = imported.PushNotifications,
                            GoalReminders = imported.GoalReminders,
                            LoanEmiReminders = imported.LoanEmiReminders,
                            InvestmentAlerts = imported.InvestmentAlerts,
                            AiAdvisorInsights = imported.AiAdvisorInsights,
                            WeeklySummary = imported.WeeklySummary,
                            MonthlyReport = imported.MonthlyReport,
                        },
                        cancellationToken);
                }
            }
        }
        catch (Exception)
        {
            return Result.Failure<UserSettingsResponse>(
                Error.Validation(
                    "Import file is invalid.",
                    new Dictionary<string, string[]> { ["contentBase64"] = ["Could not parse import file."] }));
        }

        return await GetAsync(cancellationToken);
    }

    public Task<Result> ClearCacheAsync(CancellationToken cancellationToken = default)
    {
        // Server-side cache clear placeholder — client clears localStorage.
        _ = cancellationToken;
        return Task.FromResult(Result.Success());
    }

    public async Task<Result> DeleteAccountAsync(CancellationToken cancellationToken = default)
    {
        var context = await LoadContextAsync(cancellationToken);
        if (context.IsFailure)
        {
            return Result.Failure(context.Error!);
        }

        var user = context.Value.User;
        user.IsActive = false;
        user.IsDeleted = true;
        user.DeletedAt = DateTime.UtcNow;
        _userRepository.Update(user);

        var tokens = await _refreshTokenRepository.GetActiveTokensByUserIdAsync(user.Id, cancellationToken);
        foreach (var token in tokens)
        {
            token.RevokedAt = DateTime.UtcNow;
            token.RevokedByIp = "account-deleted";
            _refreshTokenRepository.Update(token);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    private async Task<Result<(User User, UserSettings Settings)>> LoadContextAsync(
        CancellationToken cancellationToken)
    {
        if (!_currentUser.UserId.HasValue)
        {
            return Result.Failure<(User, UserSettings)>(Error.Unauthorized());
        }

        var userId = _currentUser.UserId.Value;
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user is null || user.IsDeleted || !user.IsActive)
        {
            return Result.Failure<(User, UserSettings)>(Error.NotFound(nameof(User), userId));
        }

        var settings = await _settingsRepository.GetByUserIdAsync(userId, cancellationToken);
        if (settings is null)
        {
            var workspace = string.IsNullOrWhiteSpace(user.DisplayName)
                ? $"{user.FirstName} {user.LastName}".Trim()
                : user.DisplayName!;
            settings = new UserSettings(Guid.NewGuid())
            {
                UserId = userId,
                WorkspaceName = string.IsNullOrWhiteSpace(workspace) ? string.Empty : workspace,
            };
            await _settingsRepository.AddAsync(settings, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return Result.Success((user, settings));
    }

    private static UserSettingsResponse Map(
        User user,
        UserSettings settings,
        IReadOnlyList<RefreshToken>? activeTokens = null)
    {
        var sessions = (activeTokens ?? Array.Empty<RefreshToken>())
            .OrderByDescending(token => token.CreatedAt)
            .Select(token => new ActiveSessionResponse
            {
                Id = token.Id.ToString(),
                Device = string.IsNullOrWhiteSpace(token.CreatedByIp) ? "Unknown device" : $"IP {token.CreatedByIp}",
                Location = token.CreatedByIp,
                LastActiveAt = token.CreatedAt,
                IsCurrent = false,
            })
            .ToList();

        if (sessions.Count > 0)
        {
            sessions[0].IsCurrent = true;
        }

        return new UserSettingsResponse
        {
            UserId = user.Id,
            Email = user.Email ?? string.Empty,
            FirstName = user.FirstName,
            LastName = user.LastName,
            FullName = string.IsNullOrWhiteSpace(user.DisplayName)
                ? $"{user.FirstName} {user.LastName}".Trim()
                : user.DisplayName!,
            WorkspaceName = settings.WorkspaceName,
            AvatarUrl = settings.AvatarUrl,
            Timezone = settings.Timezone,
            Country = settings.Country,
            Theme = settings.Theme,
            LayoutDensity = settings.LayoutDensity,
            SidebarCollapsed = settings.SidebarCollapsed,
            CurrencyCode = settings.CurrencyCode,
            Locale = settings.Locale,
            DateFormat = settings.DateFormat,
            NumberFormat = settings.NumberFormat,
            EmailNotifications = settings.EmailNotifications,
            PushNotifications = settings.PushNotifications,
            GoalReminders = settings.GoalReminders,
            LoanEmiReminders = settings.LoanEmiReminders,
            InvestmentAlerts = settings.InvestmentAlerts,
            AiAdvisorInsights = settings.AiAdvisorInsights,
            WeeklySummary = settings.WeeklySummary,
            MonthlyReport = settings.MonthlyReport,
            TwoFactorEnabled = settings.TwoFactorEnabled,
            AngelOneConnected = settings.AngelOneConnected,
            IndiaBondsConnected = settings.IndiaBondsConnected,
            BankSyncConnected = settings.BankSyncConnected,
            UpiConnected = settings.UpiConnected,
            ActiveSessions = sessions,
        };
    }

    private static string Normalize(string? value, string fallback) =>
        string.IsNullOrWhiteSpace(value) ? fallback : value.Trim();
}
