using FluentAssertions;
using WealthOS.Application.Investments.DTOs.Requests;
using WealthOS.Application.Investments.Validators;
using WealthOS.Domain.Investments.Enums;

namespace WealthOS.UnitTests.Investments;

public sealed class InvestmentValidatorTests
{
    [Fact]
    public void CreateInvestmentAccountRequest_WhenValid_ShouldPass()
    {
        var validator = new CreateInvestmentAccountRequestValidator();
        var result = validator.Validate(new CreateInvestmentAccountRequest
        {
            ProviderId = Guid.NewGuid(),
            Name = "Manual Investments",
            OwnerName = "Household",
            KindLabel = "SGB, FD & unlisted",
            Status = InvestmentAccountStatus.Manual,
            CurrencyCode = "INR",
        });

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void AddManualHoldingRequest_WhenNegativeAmount_ShouldFail()
    {
        var validator = new AddManualHoldingRequestValidator();
        var result = validator.Validate(new AddManualHoldingRequest
        {
            AccountId = Guid.NewGuid(),
            Name = "Test",
            Symbol = "TEST",
            Category = InvestmentCategory.Stocks,
            InvestmentType = InvestmentType.Equity,
            Quantity = 1,
            AverageCost = 10,
            InvestedAmount = -1,
            CurrentPrice = 10,
            CurrentValue = 10,
            CurrencyCode = "INR",
        });

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(AddManualHoldingRequest.InvestedAmount));
    }

    [Fact]
    public void RecordTransactionRequest_WhenMissingAccount_ShouldFail()
    {
        var validator = new RecordTransactionRequestValidator();
        var result = validator.Validate(new RecordTransactionRequest
        {
            AccountId = Guid.Empty,
            TransactionType = InvestmentTransactionType.Buy,
            Amount = 1000,
            CurrencyCode = "INR",
            TransactionDate = new DateOnly(2026, 8, 1),
        });

        result.IsValid.Should().BeFalse();
    }
}
