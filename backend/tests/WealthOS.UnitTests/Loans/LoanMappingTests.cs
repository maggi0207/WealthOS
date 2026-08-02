using AutoMapper;
using FluentAssertions;
using WealthOS.Application.Loans.DTOs.Requests;
using WealthOS.Application.Loans.DTOs.Responses;
using WealthOS.Application.Loans.Mapping;
using WealthOS.Domain.Loans.Entities;
using WealthOS.Domain.Loans.Enums;

namespace WealthOS.UnitTests.Loans;

public sealed class LoanMappingTests
{
    private readonly IMapper _mapper;

    public LoanMappingTests()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<LoanMappingProfile>());
        config.AssertConfigurationIsValid();
        _mapper = config.CreateMapper();
    }

    [Fact]
    public void Map_CreateRequest_ToLoan_ShouldMapCoreFields()
    {
        var request = new CreateLoanRequest
        {
            Name = "  Jewel loan  ",
            Type = LoanType.Jewel,
            LenderName = "  IOB  ",
            Principal = 650_000m,
            OutstandingBalance = 410_000m,
            InterestRate = 9.4m,
            EmiAmount = 18_900m,
            TenureMonths = 30,
            RemainingTenureMonths = 22,
            StartDate = new DateOnly(2024, 11, 18),
            CurrencyCode = "inr",
        };

        var loan = _mapper.Map<Loan>(request);

        loan.Name.Should().Be("Jewel loan");
        loan.LenderName.Should().Be("IOB");
        loan.CurrencyCode.Should().Be("INR");
        loan.Type.Should().Be(LoanType.Jewel);
        loan.Principal.Should().Be(650_000m);
    }

    [Fact]
    public void Map_Loan_ToResponse_ShouldMapCollections()
    {
        var loan = new Loan
        {
            Name = "Personal",
            Type = LoanType.Personal,
            LenderName = "Axis",
            Principal = 100m,
            OutstandingBalance = 50m,
            Payments =
            {
                new LoanPayment
                {
                    PaidOn = new DateOnly(2026, 7, 1),
                    Amount = 10m,
                    PrincipalComponent = 7m,
                    InterestComponent = 3m,
                    Status = LoanPaymentStatus.Paid,
                },
            },
        };

        var response = _mapper.Map<LoanResponse>(loan);

        response.Name.Should().Be("Personal");
        response.Payments.Should().HaveCount(1);
        response.Payments[0].Amount.Should().Be(10m);
    }
}
