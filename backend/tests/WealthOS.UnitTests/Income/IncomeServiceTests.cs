using AutoMapper;
using FluentAssertions;
using Moq;
using WealthOS.Application.Common.Interfaces;
using WealthOS.Application.Income.Calculations;
using WealthOS.Application.Income.DTOs.Requests;
using WealthOS.Application.Income.Mapping;
using WealthOS.Application.Income.Services;
using WealthOS.Domain.Common.Abstractions.Repositories;
using WealthOS.Domain.Income.Entities;
using WealthOS.Domain.Income.Enums;
using WealthOS.Domain.Income.Repositories;

namespace WealthOS.UnitTests.Income;

public sealed class BusinessServiceTests
{
    private readonly Mock<IBusinessClientRepository> _clientRepository = new();
    private readonly Mock<IBusinessProjectRepository> _projectRepository = new();
    private readonly Mock<IDeveloperRepository> _developerRepository = new();
    private readonly Mock<IInvoiceRepository> _invoiceRepository = new();
    private readonly Mock<IBusinessExpenseRepository> _expenseRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<ICurrentUserService> _currentUser = new();
    private readonly IMapper _mapper;
    private readonly Guid _userId = Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee");

    public BusinessServiceTests()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<IncomeMappingProfile>());
        _mapper = config.CreateMapper();

        _currentUser.SetupGet(user => user.IsAuthenticated).Returns(true);
        _currentUser.SetupGet(user => user.UserId).Returns(_userId);

        _unitOfWork
            .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
    }

    [Fact]
    public async Task CreateClientAsync_WhenAuthenticated_ShouldPersistAndReturnResponse()
    {
        BusinessClient? captured = null;
        _clientRepository
            .Setup(repo => repo.AddAsync(It.IsAny<BusinessClient>(), It.IsAny<CancellationToken>()))
            .Callback<BusinessClient, CancellationToken>((client, _) => captured = client)
            .Returns(Task.CompletedTask);

        var service = CreateService();
        var result = await service.CreateClientAsync(new CreateClientRequest
        {
            Name = "Northbridge Retail",
            Engagement = "Retainer · Web platform",
            Status = ClientStatus.Active,
            MonthlyRevenue = 275_000m,
            CurrencyCode = "INR",
        });

        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be("Northbridge Retail");
        result.Value.MonthlyRevenue.Should().Be(275_000m);
        captured.Should().NotBeNull();
        captured!.UserId.Should().Be(_userId);
        _unitOfWork.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetClientsAsync_WhenUnauthenticated_ShouldFail()
    {
        _currentUser.SetupGet(user => user.IsAuthenticated).Returns(false);
        _currentUser.SetupGet(user => user.UserId).Returns((Guid?)null);

        var service = CreateService();
        var result = await service.GetClientsAsync(1, 20, null, null);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("unauthorized");
    }

    [Fact]
    public async Task DeleteClientAsync_WhenMissing_ShouldReturnNotFound()
    {
        _clientRepository
            .Setup(repo => repo.GetByIdForUserAsync(It.IsAny<Guid>(), _userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((BusinessClient?)null);

        var service = CreateService();
        var result = await service.DeleteClientAsync(Guid.NewGuid());

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("not_found");
    }

    private BusinessService CreateService() =>
        new(
            _clientRepository.Object,
            _projectRepository.Object,
            _developerRepository.Object,
            _invoiceRepository.Object,
            _expenseRepository.Object,
            _unitOfWork.Object,
            _currentUser.Object,
            _mapper);
}

public sealed class IncomeServiceTests
{
    private readonly Mock<ISalaryRepository> _salaryRepository = new();
    private readonly Mock<IInvoiceRepository> _invoiceRepository = new();
    private readonly Mock<IDeveloperRepository> _developerRepository = new();
    private readonly Mock<IBusinessExpenseRepository> _expenseRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<ICurrentUserService> _currentUser = new();
    private readonly IMapper _mapper;
    private readonly Guid _userId = Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee");

    public IncomeServiceTests()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<IncomeMappingProfile>());
        _mapper = config.CreateMapper();

        _currentUser.SetupGet(user => user.IsAuthenticated).Returns(true);
        _currentUser.SetupGet(user => user.UserId).Returns(_userId);

        _unitOfWork
            .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
    }

    [Fact]
    public async Task GetDashboardAsync_ShouldAggregatePeriodTotals()
    {
        _salaryRepository
            .Setup(repo => repo.SumPaymentsForPeriodAsync(_userId, "2026-07", It.IsAny<CancellationToken>()))
            .ReturnsAsync(385_000m);
        _invoiceRepository
            .Setup(repo => repo.SumPaymentsForPeriodAsync(_userId, "2026-07", It.IsAny<CancellationToken>()))
            .ReturnsAsync(640_000m);
        _developerRepository
            .Setup(repo => repo.SumPayrollForPeriodAsync(_userId, "2026-07", It.IsAny<CancellationToken>()))
            .ReturnsAsync(310_000m);
        _expenseRepository
            .Setup(repo => repo.SumForPeriodAsync(_userId, "2026-07", It.IsAny<CancellationToken>()))
            .ReturnsAsync(78_500m);
        _invoiceRepository
            .Setup(repo => repo.SumOutstandingAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(398_000m);

        var service = CreateService();
        var result = await service.GetDashboardAsync("2026-07");

        result.IsSuccess.Should().BeTrue();
        result.Value.Salary.Should().Be(385_000m);
        result.Value.BusinessRevenue.Should().Be(640_000m);
        result.Value.OutstandingInvoices.Should().Be(398_000m);
        result.Value.NetProfit.Should().Be(251_500m);
    }

    private IncomeService CreateService() =>
        new(
            _salaryRepository.Object,
            _invoiceRepository.Object,
            _developerRepository.Object,
            _expenseRepository.Object,
            _unitOfWork.Object,
            _currentUser.Object,
            new IncomeCalculationService(),
            _mapper);
}

public sealed class InvoiceServiceTests
{
    private readonly Mock<IInvoiceRepository> _invoiceRepository = new();
    private readonly Mock<IBusinessClientRepository> _clientRepository = new();
    private readonly Mock<IBusinessProjectRepository> _projectRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<ICurrentUserService> _currentUser = new();
    private readonly IMapper _mapper;
    private readonly Guid _userId = Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee");

    public InvoiceServiceTests()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<IncomeMappingProfile>());
        _mapper = config.CreateMapper();

        _currentUser.SetupGet(user => user.IsAuthenticated).Returns(true);
        _currentUser.SetupGet(user => user.UserId).Returns(_userId);

        _unitOfWork
            .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
    }

    [Fact]
    public async Task RecordPaymentAsync_WhenExceedsOutstanding_ShouldFailValidation()
    {
        var invoiceId = Guid.NewGuid();
        var invoice = new Invoice(invoiceId)
        {
            UserId = _userId,
            ClientId = Guid.NewGuid(),
            InvoiceNumber = "INV-1",
            IssueDate = new DateOnly(2026, 7, 1),
            DueDate = new DateOnly(2026, 7, 15),
            Status = InvoiceStatus.Sent,
            SubTotal = 100_000m,
            AmountPaid = 80_000m,
        };

        _invoiceRepository
            .Setup(repo => repo.GetByIdWithDetailsAsync(invoiceId, _userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(invoice);

        var service = new InvoiceService(
            _invoiceRepository.Object,
            _clientRepository.Object,
            _projectRepository.Object,
            _unitOfWork.Object,
            _currentUser.Object,
            _mapper);

        var result = await service.RecordPaymentAsync(new RecordInvoicePaymentRequest
        {
            InvoiceId = invoiceId,
            Amount = 30_000m,
            PaidOn = new DateOnly(2026, 7, 20),
        });

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("validation_error");
    }
}
