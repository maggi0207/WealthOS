namespace WealthOS.Infrastructure.Investments.Providers;

/// <summary>
/// Angel One SmartAPI configuration. Secrets must come from environment / vault — never commit.
/// Trading endpoints are intentionally unsupported.
/// </summary>
public sealed class AngelOneOptions
{
    public const string SectionName = "AngelOne";

    /// <summary>SmartAPI base URL (read-only market/portfolio APIs).</summary>
    public string BaseUrl { get; set; } = "https://apiconnect.angelone.in";

    /// <summary>API key from Angel One SmartAPI developer portal.</summary>
    public string? ApiKey { get; set; }

    /// <summary>Client code / user id.</summary>
    public string? ClientCode { get; set; }

    /// <summary>Trading PIN used by loginByPassword (not a trading API).</summary>
    public string? Password { get; set; }

    /// <summary>Base32 TOTP secret from SmartAPI “Enable TOTP”.</summary>
    public string? TotpSecret { get; set; }

    /// <summary>Optional vault reference for refresh token (future).</summary>
    public string? RefreshTokenSecretName { get; set; }

    /// <summary>When false, sync methods become idempotent no-ops that touch LastSyncedAt only.</summary>
    public bool EnableLiveSync { get; set; }

    /// <summary>Headers required by SmartAPI.</summary>
    public string ClientLocalIp { get; set; } = "127.0.0.1";

    public string ClientPublicIp { get; set; } = "127.0.0.1";

    public string MacAddress { get; set; } = "00:00:00:00:00:00";
}
