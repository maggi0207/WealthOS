namespace WealthOS.Application.Authentication.Options;

public sealed class AdminSeedSettings
{
    public const string SectionName = "AdminSeed";

    public string Email { get; set; } = "admin@wealthos.local";

    public string Password { get; set; } = string.Empty;

    public string FirstName { get; set; } = "System";

    public string LastName { get; set; } = "Admin";
}
