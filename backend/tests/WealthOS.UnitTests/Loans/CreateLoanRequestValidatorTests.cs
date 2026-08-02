using FluentAssertions;
using WealthOS.Application.Loans.DTOs.Requests;
using WealthOS.Application.Loans.Validators;
using WealthOS.Domain.Loans.Enums;

namespace WealthOS.UnitTests.Loans;

public sealed class CreateLoanRequestValidatorTests
{
    private readonly CreateLoanRequestValidator _validator = new();

    [Fact]
    public void Validate_WhenValid_ShouldSucceed()
    {
        var request = new CreateLoanRequest
        {
            Name = "Home loan — Ramana Flats",
            Type = LoanType.Home,
            LenderName = "HDFC Bank",
            Principal = 6_200_000m,
            OutstandingBalance = 3_845_000m,
            InterestRate = 8.6m,
            EmiAmount = 52_400m,
            TenureMonths = 180,
            RemainingTenureMonths = 82,
            StartDate = new DateOnly(2018, 6, 5),
            EndDate = new DateOnly(2033, 5, 5),
            CurrencyCode = "INR",
            Status = LoanStatus.Active,
        };

        var result = _validator.Validate(request);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WhenNameEmpty_ShouldFail()
    {
        var request = new CreateLoanRequest
        {
            Name = "",
            Type = LoanType.Personal,
            LenderName = "Bank",
            Principal = 100m,
            OutstandingBalance = 50m,
            TenureMonths = 12,
            RemainingTenureMonths = 6,
            StartDate = new DateOnly(2026, 1, 1),
            CurrencyCode = "INR",
        };

        var result = _validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(error => error.PropertyName == nameof(CreateLoanRequest.Name));
    }

    [Fact]
    public void Validate_WhenOutstandingExceedsPrincipal_ShouldFail()
    {
        var request = new CreateLoanRequest
        {
            Name = "Bad loan",
            Type = LoanType.Other,
            LenderName = "Bank",
            Principal = 100m,
            OutstandingBalance = 150m,
            TenureMonths = 12,
            RemainingTenureMonths = 12,
            StartDate = new DateOnly(2026, 1, 1),
            CurrencyCode = "INR",
        };

        var result = _validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(error =>
            error.ErrorMessage.Contains("Outstanding balance", StringComparison.OrdinalIgnoreCase));
    }
}
