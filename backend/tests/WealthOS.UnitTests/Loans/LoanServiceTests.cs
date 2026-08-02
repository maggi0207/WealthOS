using AutoMapper;
using FluentAssertions;
using Moq;
using WealthOS.Application.Common.Interfaces;
using WealthOS.Application.Loans.Calculations;
using WealthOS.Application.Loans.DTOs.Requests;
using WealthOS.Application.Loans.Mapping;
using WealthOS.Application.Loans.Services;
using WealthOS.Domain.Common.Abstractions.Repositories;
using WealthOS.Domain.Loans.Entities;
using WealthOS.Domain.Loans.Enums;
using WealthOS.Domain.Loans.Models;
using WealthOS.Domain.Loans.Repositories;
using WealthOS.Domain.Properties.Repositories;

namespace WealthOS.UnitTests.Loans;

public sealed class LoanServiceTests
{
    private readonly Mock<ILoanRepository> _loanRepository = new();
    private readonly Mock<ILoanProviderRepository> _providerRepository = new();
    private readonly Mock<IPropertyRepository> _propertyRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<ICurrentUserService> _currentUser = new();
    private readonly IMapper _mapper;
    private readonly Guid _userId = Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee");

    public LoanServiceTests()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<LoanMappingProfile>());
        _mapper = config.CreateMapper();

        _currentUser.SetupGet(user => user.IsAuthenticated).Returns(true);
        _currentUser.SetupGet(user => user.UserId).Returns(_userId);

        _unitOfWork
            .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _providerRepository
            .Setup(repo => repo.FindByNameForUserAsync(
                _userId,
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((LoanProvider?)null);

        _providerRepository
            .Setup(repo => repo.AddAsync(It.IsAny<LoanProvider>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
    }

    [Fact]
    public async Task GetSummaryAsync_WhenAuthenticated_ShouldMapPortfolioTotals()
    {
        _loanRepository
            .Setup(repo => repo.GetPortfolioSummaryAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new LoanSummary
            {
                LoanCount = 3,
                TotalLoanAmount = 7_650_000m,
                OutstandingBalance = 4_547_000m,
                MonthlyEmi = 92_900m,
                UpcomingEmi = 92_900m,
                CurrencyCode = "INR",
                ActiveCount = 3,
                ClosedCount = 0,
            });

        var service = CreateService();
        var result = await service.GetSummaryAsync();

        result.IsSuccess.Should().BeTrue();
        result.Value.LoanCount.Should().Be(3);
        result.Value.OutstandingBalance.Should().Be(4_547_000m);
        result.Value.MonthlyEmi.Should().Be(92_900m);
        result.Value.UpcomingEmi.Should().Be(92_900m);
    }

    [Fact]
    public async Task GetByIdAsync_WhenMissing_ShouldReturnNotFound()
    {
        var loanId = Guid.NewGuid();
        _loanRepository
            .Setup(repo => repo.GetByIdWithDetailsAsync(loanId, _userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Loan?)null);

        var service = CreateService();
        var result = await service.GetByIdAsync(loanId);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("not_found");
    }

    [Fact]
    public async Task CreateAsync_WhenUnauthenticated_ShouldFailUnauthorized()
    {
        _currentUser.SetupGet(user => user.IsAuthenticated).Returns(false);
        _currentUser.SetupGet(user => user.UserId).Returns((Guid?)null);

        var service = CreateService();
        var result = await service.CreateAsync(ValidCreateRequest());

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("unauthorized");
    }

    [Fact]
    public async Task DeleteAsync_WhenExists_ShouldSoftDeleteViaRepository()
    {
        var loanId = Guid.NewGuid();
        var loan = new Loan(loanId) { UserId = _userId, Name = "To delete" };

        _loanRepository
            .Setup(repo => repo.GetByIdForUserAsync(loanId, _userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(loan);

        var service = CreateService();
        var result = await service.DeleteAsync(loanId);

        result.IsSuccess.Should().BeTrue();
        _loanRepository.Verify(repo => repo.Remove(loan), Times.Once);
        _unitOfWork.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RecordPaymentAsync_WhenPaid_ShouldReduceOutstanding()
    {
        var loanId = Guid.NewGuid();
        var loan = new Loan(loanId)
        {
            UserId = _userId,
            Name = "Home",
            Principal = 1_000_000m,
            OutstandingBalance = 500_000m,
            RemainingTenureMonths = 20,
            NextEmiDate = new DateOnly(2026, 8, 5),
            PaymentFrequency = PaymentFrequency.Monthly,
            Status = LoanStatus.Active,
            EmiAmount = 10_000m,
        };

        _loanRepository
            .Setup(repo => repo.GetByIdWithDetailsAsync(loanId, _userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(loan);

        var service = CreateService();
        var result = await service.RecordPaymentAsync(loanId, new RecordLoanPaymentRequest
        {
            PaidOn = new DateOnly(2026, 8, 5),
            Amount = 10_000m,
            PrincipalComponent = 6_000m,
            InterestComponent = 4_000m,
            Status = LoanPaymentStatus.Paid,
            ApplyToOutstanding = true,
        });

        result.IsSuccess.Should().BeTrue();
        loan.OutstandingBalance.Should().Be(494_000m);
        loan.RemainingTenureMonths.Should().Be(19);
        loan.NextEmiDate.Should().Be(new DateOnly(2026, 9, 5));
        loan.Payments.Should().HaveCount(1);
    }

    private LoanService CreateService() =>
        new(
            _loanRepository.Object,
            _providerRepository.Object,
            _propertyRepository.Object,
            _unitOfWork.Object,
            _currentUser.Object,
            new LoanCalculationService(),
            _mapper);

    private static CreateLoanRequest ValidCreateRequest() =>
        new()
        {
            Name = "Test loan",
            Type = LoanType.Personal,
            LenderName = "Test Bank",
            Principal = 100_000m,
            OutstandingBalance = 80_000m,
            InterestRate = 12m,
            EmiAmount = 5_000m,
            TenureMonths = 24,
            RemainingTenureMonths = 18,
            StartDate = new DateOnly(2025, 1, 1),
            CurrencyCode = "INR",
        };
}
