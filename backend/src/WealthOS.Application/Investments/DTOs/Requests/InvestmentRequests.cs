using WealthOS.Domain.Investments.Enums;

namespace WealthOS.Application.Investments.DTOs.Requests;

/// <summary>
/// Creates a user investment account linked to a provider.
/// </summary>
public sealed class CreateInvestmentAccountRequest
{
    public Guid ProviderId { get; init; }

    public string Name { get; init; } = string.Empty;

    public string OwnerName { get; init; } = string.Empty;

    public string KindLabel { get; init; } = string.Empty;

    public InvestmentAccountStatus Status { get; init; } = InvestmentAccountStatus.Manual;

    public string CurrencyCode { get; init; } = "INR";

    public string? Notes { get; init; }

    public string? ExternalAccountReference { get; init; }
}

/// <summary>
/// Updates an existing investment account.
/// </summary>
public sealed class UpdateInvestmentAccountRequest
{
    public string Name { get; init; } = string.Empty;

    public string OwnerName { get; init; } = string.Empty;

    public string KindLabel { get; init; } = string.Empty;

    public InvestmentAccountStatus Status { get; init; }

    public string CurrencyCode { get; init; } = "INR";

    public string? Notes { get; init; }

    public string? ExternalAccountReference { get; init; }
}

/// <summary>
/// Adds a manually tracked holding to an account.
/// </summary>
public sealed class AddManualHoldingRequest
{
    public Guid AccountId { get; init; }

    public string Name { get; init; } = string.Empty;

    public string Symbol { get; init; } = string.Empty;

    public InvestmentCategory Category { get; init; }

    public InvestmentType InvestmentType { get; init; }

    public decimal Quantity { get; init; }

    public decimal AverageCost { get; init; }

    public decimal InvestedAmount { get; init; }

    public decimal CurrentPrice { get; init; }

    public decimal CurrentValue { get; init; }

    public decimal DayChange { get; init; }

    public decimal DayChangePercent { get; init; }

    public string CurrencyCode { get; init; } = "INR";

    public string? Notes { get; init; }
}

/// <summary>
/// Updates an existing holding.
/// </summary>
public sealed class UpdateHoldingRequest
{
    public string Name { get; init; } = string.Empty;

    public string Symbol { get; init; } = string.Empty;

    public InvestmentCategory Category { get; init; }

    public InvestmentType InvestmentType { get; init; }

    public decimal Quantity { get; init; }

    public decimal AverageCost { get; init; }

    public decimal InvestedAmount { get; init; }

    public decimal CurrentPrice { get; init; }

    public decimal CurrentValue { get; init; }

    public decimal DayChange { get; init; }

    public decimal DayChangePercent { get; init; }

    public string CurrencyCode { get; init; } = "INR";

    public string? Notes { get; init; }
}

/// <summary>
/// Records a transaction against an account / optional holding.
/// </summary>
public sealed class RecordTransactionRequest
{
    public Guid AccountId { get; init; }

    public Guid? HoldingId { get; init; }

    public InvestmentTransactionType TransactionType { get; init; }

    public decimal Quantity { get; init; }

    public decimal Price { get; init; }

    public decimal Amount { get; init; }

    public decimal Fees { get; init; }

    public DateOnly TransactionDate { get; init; }

    public string CurrencyCode { get; init; } = "INR";

    public string? Notes { get; init; }

    public string? ExternalReference { get; init; }
}

/// <summary>
/// Requests a provider connect / sync action (stub for non-manual providers).
/// </summary>
public sealed class ProviderConnectRequest
{
    public Guid AccountId { get; init; }
}
