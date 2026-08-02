using FluentAssertions;
using FluentValidation.TestHelper;
using WealthOS.Application.Income.DTOs.Requests;
using WealthOS.Application.Income.Validators;
using WealthOS.Domain.Income.Enums;

namespace WealthOS.UnitTests.Income;

public sealed class IncomeValidatorTests
{
    [Fact]
    public void CreateClientRequest_WhenValid_ShouldPass()
    {
        var validator = new CreateClientRequestValidator();
        var result = validator.TestValidate(new CreateClientRequest
        {
            Name = "Northbridge Retail",
            Engagement = "Retainer · Web platform",
            Status = ClientStatus.Active,
            MonthlyRevenue = 275_000m,
            CurrencyCode = "INR",
        });

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void CreateInvoiceRequest_WhenNoItems_ShouldFail()
    {
        var validator = new CreateInvoiceRequestValidator();
        var result = validator.TestValidate(new CreateInvoiceRequest
        {
            ClientId = Guid.NewGuid(),
            InvoiceNumber = "INV-1",
            IssueDate = new DateOnly(2026, 7, 1),
            DueDate = new DateOnly(2026, 7, 15),
            Items = [],
        });

        result.ShouldHaveValidationErrorFor(x => x.Items);
    }

    [Fact]
    public void RecordSalaryRequest_WhenPeriodInvalid_ShouldFail()
    {
        var validator = new RecordSalaryRequestValidator();
        var result = validator.TestValidate(new RecordSalaryRequest
        {
            MemberName = "Magesh",
            Employer = "Zoho",
            Role = "EM",
            MonthlyAmount = 245_000m,
            PaidOn = new DateOnly(2026, 7, 31),
            Period = "2026/07",
        });

        result.ShouldHaveValidationErrorFor(x => x.Period);
    }

    [Fact]
    public void CreatePayrollRequest_WhenAmountZero_ShouldFail()
    {
        var validator = new CreatePayrollRequestValidator();
        var result = validator.TestValidate(new CreatePayrollRequest
        {
            DeveloperId = Guid.NewGuid(),
            Amount = 0m,
            Period = "2026-07",
            Status = PayrollStatus.Pending,
        });

        result.ShouldHaveValidationErrorFor(x => x.Amount);
    }
}
