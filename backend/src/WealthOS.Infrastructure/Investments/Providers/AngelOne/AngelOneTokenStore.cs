namespace WealthOS.Infrastructure.Investments.Providers;

/// <summary>
/// In-memory secure token holder for Angel One session tokens.
/// Never logs token values.
/// </summary>
public sealed class AngelOneTokenStore
{
    private readonly object _gate = new();
    private string? _accessToken;
    private string? _refreshToken;
    private DateTimeOffset? _expiresAt;

    public bool HasAccessToken
    {
        get
        {
            lock (_gate)
            {
                return !string.IsNullOrWhiteSpace(_accessToken)
                    && (_expiresAt is null || _expiresAt > DateTimeOffset.UtcNow.AddMinutes(1));
            }
        }
    }

    public void SetTokens(string accessToken, string? refreshToken, DateTimeOffset? expiresAt)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(accessToken);
        lock (_gate)
        {
            _accessToken = accessToken;
            _refreshToken = refreshToken;
            _expiresAt = expiresAt;
        }
    }

    public string? GetAccessToken()
    {
        lock (_gate)
        {
            return _accessToken;
        }
    }

    public string? GetRefreshToken()
    {
        lock (_gate)
        {
            return _refreshToken;
        }
    }

    public void Clear()
    {
        lock (_gate)
        {
            _accessToken = null;
            _refreshToken = null;
            _expiresAt = null;
        }
    }
}
