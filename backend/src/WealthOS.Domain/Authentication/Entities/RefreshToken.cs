using WealthOS.Domain.Common.Entities;

namespace WealthOS.Domain.Authentication.Entities;

public sealed class RefreshToken : BaseEntity
{
    public RefreshToken()
        : base(Guid.NewGuid())
    {
    }

    public Guid UserId { get; set; }

    public User User { get; set; } = null!;

    public string Token { get; set; } = string.Empty;

    public DateTime ExpiresAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public string CreatedByIp { get; set; } = string.Empty;

    public DateTime? RevokedAt { get; set; }

    public string? RevokedByIp { get; set; }

    public string? ReplacedByToken { get; set; }

    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;

    public bool IsRevoked => RevokedAt is not null;

    public bool IsActive => !IsRevoked && !IsExpired;
}
