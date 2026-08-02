namespace WealthOS.Application.Authentication.DTOs.Responses;

public sealed record UserProfileResponse
{
    public Guid Id { get; init; }

    public string Email { get; init; } = string.Empty;

    public string FirstName { get; init; } = string.Empty;

    public string LastName { get; init; } = string.Empty;

    public string? DisplayName { get; init; }

    public IReadOnlyList<string> Roles { get; init; } = [];

    public bool EmailConfirmed { get; init; }

    public bool IsActive { get; init; }
}
