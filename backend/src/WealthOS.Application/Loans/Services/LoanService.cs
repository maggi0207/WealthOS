using AutoMapper;
using WealthOS.Application.Common.Interfaces;
using WealthOS.Application.Common.Models;
using WealthOS.Application.Loans.Calculations;
using WealthOS.Application.Loans.DTOs.Requests;
using WealthOS.Application.Loans.DTOs.Responses;
using WealthOS.Application.Loans.Interfaces;
using WealthOS.Domain.Common.Abstractions.Repositories;
using WealthOS.Domain.Loans.Entities;
using WealthOS.Domain.Loans.Enums;
using WealthOS.Domain.Loans.Repositories;
using WealthOS.Domain.Properties.Repositories;

namespace WealthOS.Application.Loans.Services;

/// <summary>
/// Orchestrates loan CRUD, payments, summary, upcoming, and dashboard use cases.
/// </summary>
public sealed class LoanService : ILoanService
{
    private readonly ILoanRepository _loanRepository;
    private readonly ILoanProviderRepository _loanProviderRepository;
    private readonly IPropertyRepository _propertyRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;
    private readonly ILoanCalculationService _calculator;
    private readonly IMapper _mapper;

    public LoanService(
        ILoanRepository loanRepository,
        ILoanProviderRepository loanProviderRepository,
        IPropertyRepository propertyRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser,
        ILoanCalculationService calculator,
        IMapper mapper)
    {
        _loanRepository = loanRepository;
        _loanProviderRepository = loanProviderRepository;
        _propertyRepository = propertyRepository;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _calculator = calculator;
        _mapper = mapper;
    }

    public async Task<Result<LoanResponse>> CreateAsync(
        CreateLoanRequest request,
        CancellationToken cancellationToken = default)
    {
        var userResult = RequireUserId();
        if (userResult.IsFailure)
        {
            return Result.Failure<LoanResponse>(userResult.Error!);
        }

        var linkResult = await ValidateLinkedPropertyAsync(
            request.LinkedPropertyId,
            userResult.Value,
            cancellationToken);
        if (linkResult.IsFailure)
        {
            return Result.Failure<LoanResponse>(linkResult.Error!);
        }

        var providerResult = await EnsureProviderAsync(
            userResult.Value,
            request.LoanProviderId,
            request.LenderName,
            cancellationToken);
        if (providerResult.IsFailure)
        {
            return Result.Failure<LoanResponse>(providerResult.Error!);
        }

        var loan = _mapper.Map<Loan>(request);
        loan.UserId = userResult.Value;
        loan.LoanProviderId = providerResult.Value;

        loan.InterestRates.Add(new LoanInterestRate
        {
            RatePercent = loan.InterestRate,
            InterestType = loan.InterestType,
            EffectiveFrom = loan.StartDate,
            Reason = "Initial rate",
        });

        if (loan.LinkedPropertyId.HasValue)
        {
            loan.PropertyLinks.Add(new LoanPropertyLink
            {
                PropertyId = loan.LinkedPropertyId.Value,
                IsPrimary = true,
            });
        }

        if (loan.NextEmiDate.HasValue && loan.EmiAmount > 0 && loan.Status == LoanStatus.Active)
        {
            loan.Reminders.Add(new LoanReminder
            {
                Title = $"{loan.Type} loan EMI",
                Detail = loan.AutoDebit
                    ? $"Auto debit · {loan.LenderName}"
                    : $"Manual · {loan.LenderName}",
                DueOn = loan.NextEmiDate.Value,
                Amount = loan.EmiAmount,
                IsUrgent = loan.NextEmiDate.Value <= DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7)),
            });
        }

        await _loanRepository.AddAsync(loan, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var created = await _loanRepository.GetByIdWithDetailsAsync(
            loan.Id,
            userResult.Value,
            cancellationToken);

        return Result.Success(MapDetail(created!));
    }

    public async Task<Result<LoanResponse>> UpdateAsync(
        Guid loanId,
        UpdateLoanRequest request,
        CancellationToken cancellationToken = default)
    {
        var userResult = RequireUserId();
        if (userResult.IsFailure)
        {
            return Result.Failure<LoanResponse>(userResult.Error!);
        }

        var loan = await _loanRepository.GetByIdWithDetailsAsync(
            loanId,
            userResult.Value,
            cancellationToken);

        if (loan is null)
        {
            return Result.Failure<LoanResponse>(Error.NotFound(nameof(Loan), loanId));
        }

        var linkResult = await ValidateLinkedPropertyAsync(
            request.LinkedPropertyId,
            userResult.Value,
            cancellationToken);
        if (linkResult.IsFailure)
        {
            return Result.Failure<LoanResponse>(linkResult.Error!);
        }

        var providerResult = await EnsureProviderAsync(
            userResult.Value,
            request.LoanProviderId,
            request.LenderName,
            cancellationToken);
        if (providerResult.IsFailure)
        {
            return Result.Failure<LoanResponse>(providerResult.Error!);
        }

        var rateChanged = loan.InterestRate != request.InterestRate
            || loan.InterestType != request.InterestType;

        ApplyUpdate(loan, request, providerResult.Value);

        if (rateChanged)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            foreach (var openRate in loan.InterestRates.Where(rate => rate.EffectiveTo is null))
            {
                openRate.EffectiveTo = today;
            }

            loan.InterestRates.Add(new LoanInterestRate
            {
                RatePercent = loan.InterestRate,
                InterestType = loan.InterestType,
                EffectiveFrom = today,
                Reason = "Rate update",
            });
        }

        SyncPrimaryPropertyLink(loan);

        _loanRepository.Update(loan);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var updated = await _loanRepository.GetByIdWithDetailsAsync(
            loanId,
            userResult.Value,
            cancellationToken);

        return Result.Success(MapDetail(updated!));
    }

    public async Task<Result> DeleteAsync(Guid loanId, CancellationToken cancellationToken = default)
    {
        var userResult = RequireUserId();
        if (userResult.IsFailure)
        {
            return Result.Failure(userResult.Error!);
        }

        var loan = await _loanRepository.GetByIdForUserAsync(
            loanId,
            userResult.Value,
            cancellationToken);

        if (loan is null)
        {
            return Result.Failure(Error.NotFound(nameof(Loan), loanId));
        }

        _loanRepository.Remove(loan);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result<LoanPaymentResponse>> RecordPaymentAsync(
        Guid loanId,
        RecordLoanPaymentRequest request,
        CancellationToken cancellationToken = default)
    {
        var userResult = RequireUserId();
        if (userResult.IsFailure)
        {
            return Result.Failure<LoanPaymentResponse>(userResult.Error!);
        }

        var loan = await _loanRepository.GetByIdWithDetailsAsync(
            loanId,
            userResult.Value,
            cancellationToken);

        if (loan is null)
        {
            return Result.Failure<LoanPaymentResponse>(Error.NotFound(nameof(Loan), loanId));
        }

        var payment = new LoanPayment
        {
            PaidOn = request.PaidOn,
            Amount = request.Amount,
            PrincipalComponent = request.PrincipalComponent,
            InterestComponent = request.InterestComponent,
            Status = request.Status,
            PaymentMode = request.PaymentMode,
            Reference = request.Reference,
            Notes = request.Notes,
            IsPrepayment = request.IsPrepayment,
        };

        loan.Payments.Add(payment);

        if (request.ApplyToOutstanding
            && request.Status == LoanPaymentStatus.Paid
            && request.PrincipalComponent > 0)
        {
            loan.OutstandingBalance = _calculator.CalculateOutstandingAfterPrincipalPayment(
                loan.OutstandingBalance,
                request.PrincipalComponent);

            if (loan.RemainingTenureMonths > 0 && !request.IsPrepayment)
            {
                loan.RemainingTenureMonths -= 1;
            }

            if (loan.OutstandingBalance <= 0)
            {
                loan.OutstandingBalance = 0;
                loan.RemainingTenureMonths = 0;
                loan.Status = LoanStatus.Closed;
                loan.NextEmiDate = null;
            }
            else if (loan.NextEmiDate.HasValue && loan.PaymentFrequency == PaymentFrequency.Monthly)
            {
                loan.NextEmiDate = loan.NextEmiDate.Value.AddMonths(1);
            }
        }

        _loanRepository.Update(loan);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(_mapper.Map<LoanPaymentResponse>(payment));
    }

    public async Task<Result<LoanResponse>> GetByIdAsync(
        Guid loanId,
        CancellationToken cancellationToken = default)
    {
        var userResult = RequireUserId();
        if (userResult.IsFailure)
        {
            return Result.Failure<LoanResponse>(userResult.Error!);
        }

        var loan = await _loanRepository.GetByIdWithDetailsAsync(
            loanId,
            userResult.Value,
            cancellationToken);

        if (loan is null)
        {
            return Result.Failure<LoanResponse>(Error.NotFound(nameof(Loan), loanId));
        }

        return Result.Success(MapDetail(loan));
    }

    public async Task<Result<LoanListResponse>> GetAllAsync(
        int page,
        int pageSize,
        string? search,
        LoanStatus? status,
        LoanType? type,
        CancellationToken cancellationToken = default)
    {
        var userResult = RequireUserId();
        if (userResult.IsFailure)
        {
            return Result.Failure<LoanListResponse>(userResult.Error!);
        }

        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var (items, totalCount) = await _loanRepository.ListForUserAsync(
            userResult.Value,
            page,
            pageSize,
            search,
            status,
            type,
            cancellationToken);

        var response = new LoanListResponse
        {
            Items = items.Select(MapListItem).ToList(),
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = totalCount == 0 ? 0 : (int)Math.Ceiling(totalCount / (double)pageSize),
        };

        return Result.Success(response);
    }

    public async Task<Result<LoanSummaryResponse>> GetSummaryAsync(
        CancellationToken cancellationToken = default)
    {
        var userResult = RequireUserId();
        if (userResult.IsFailure)
        {
            return Result.Failure<LoanSummaryResponse>(userResult.Error!);
        }

        var summary = await _loanRepository.GetPortfolioSummaryAsync(
            userResult.Value,
            cancellationToken);

        return Result.Success(new LoanSummaryResponse
        {
            LoanCount = summary.LoanCount,
            TotalLoanAmount = summary.TotalLoanAmount,
            OutstandingBalance = summary.OutstandingBalance,
            MonthlyEmi = summary.MonthlyEmi,
            UpcomingEmi = summary.UpcomingEmi,
            CurrencyCode = summary.CurrencyCode,
            ActiveCount = summary.ActiveCount,
            ClosedCount = summary.ClosedCount,
        });
    }

    public async Task<Result<UpcomingPaymentsResponse>> GetUpcomingPaymentsAsync(
        int daysAhead = 45,
        int take = 20,
        CancellationToken cancellationToken = default)
    {
        var userResult = RequireUserId();
        if (userResult.IsFailure)
        {
            return Result.Failure<UpcomingPaymentsResponse>(userResult.Error!);
        }

        daysAhead = Math.Clamp(daysAhead, 1, 365);
        take = Math.Clamp(take, 1, 100);

        var reminders = await _loanRepository.GetUpcomingRemindersAsync(
            userResult.Value,
            daysAhead,
            take,
            cancellationToken);

        var items = reminders.Select(reminder =>
        {
            var mapped = _mapper.Map<LoanReminderResponse>(reminder);
            mapped.LoanName = reminder.Loan?.Name ?? string.Empty;
            return mapped;
        }).ToList();

        // Fallback: synthesize from active loans when no reminder rows exist yet.
        if (items.Count == 0)
        {
            var loans = await _loanRepository.GetActiveLoansWithNextEmiAsync(
                userResult.Value,
                cancellationToken);

            var horizon = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(daysAhead));
            items = loans
                .Where(loan => loan.NextEmiDate.HasValue && loan.NextEmiDate.Value <= horizon)
                .OrderBy(loan => loan.NextEmiDate)
                .Take(take)
                .Select(loan => new LoanReminderResponse
                {
                    Id = Guid.Empty,
                    LoanId = loan.Id,
                    LoanName = loan.Name,
                    Title = $"{loan.Type} loan EMI",
                    Detail = loan.AutoDebit
                        ? $"Auto debit · {loan.LenderName}"
                        : $"Manual · {loan.LenderName}",
                    DueOn = loan.NextEmiDate!.Value,
                    Amount = loan.EmiAmount,
                    IsUrgent = loan.NextEmiDate.Value <= DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7)),
                })
                .ToList();
        }

        return Result.Success(new UpcomingPaymentsResponse
        {
            Items = items,
            TotalUpcomingAmount = items.Sum(item => item.Amount),
            CurrencyCode = "INR",
        });
    }

    public async Task<Result<LoanDashboardResponse>> GetDashboardAsync(
        Guid loanId,
        CancellationToken cancellationToken = default)
    {
        var detailResult = await GetByIdAsync(loanId, cancellationToken);
        if (detailResult.IsFailure)
        {
            return Result.Failure<LoanDashboardResponse>(detailResult.Error!);
        }

        var loan = detailResult.Value;
        var sampleLump = Math.Min(100_000m, loan.OutstandingBalance);
        LoanPrepaymentScenarioResponse? sample = null;

        if (sampleLump > 0 && loan.EmiAmount > 0)
        {
            var estimate = _calculator.EstimatePrepayment(
                loan.OutstandingBalance,
                loan.EmiAmount,
                loan.InterestRate,
                loan.RemainingTenureMonths,
                sampleLump);

            sample = new LoanPrepaymentScenarioResponse
            {
                LoanId = loan.Id,
                LumpSum = sampleLump,
                CurrentOutstanding = loan.OutstandingBalance,
                NewOutstanding = estimate.NewOutstanding,
                CurrentRemainingMonths = loan.RemainingTenureMonths,
                EstimatedRemainingMonths = estimate.EstimatedRemainingMonths,
                MonthsSaved = estimate.MonthsSaved,
                EstimatedInterestSaved = estimate.EstimatedInterestSaved,
                CalculatorKey = estimate.CalculatorKey,
            };
        }

        return Result.Success(new LoanDashboardResponse
        {
            Loan = loan,
            TotalPrincipalPaid = loan.TotalPrincipalPaid,
            TotalInterestPaid = loan.TotalInterestPaid,
            LoanProgressPercent = loan.LoanProgressPercent,
            EmiProgressPercent = loan.EmiProgressPercent,
            PaymentCount = loan.Payments.Count,
            ReminderCount = loan.Reminders.Count,
            DocumentLinkCount = loan.DocumentLinks.Count,
            PropertyLinkCount = loan.PropertyLinks.Count,
            SamplePrepayment = sample,
            GeneratedAt = DateTime.UtcNow,
        });
    }

    private Result<Guid> RequireUserId()
    {
        if (!_currentUser.IsAuthenticated || _currentUser.UserId is null)
        {
            return Result.Failure<Guid>(Error.Unauthorized());
        }

        return Result.Success(_currentUser.UserId.Value);
    }

    private async Task<Result> ValidateLinkedPropertyAsync(
        Guid? propertyId,
        Guid userId,
        CancellationToken cancellationToken)
    {
        if (!propertyId.HasValue)
        {
            return Result.Success();
        }

        var exists = await _propertyRepository.ExistsForUserAsync(
            propertyId.Value,
            userId,
            cancellationToken);

        return exists
            ? Result.Success()
            : Result.Failure(Error.NotFound("Property", propertyId.Value));
    }

    private async Task<Result<Guid?>> EnsureProviderAsync(
        Guid userId,
        Guid? providerId,
        string lenderName,
        CancellationToken cancellationToken)
    {
        if (providerId.HasValue)
        {
            var existing = await _loanProviderRepository.GetByIdForUserAsync(
                providerId.Value,
                userId,
                cancellationToken);

            return existing is null
                ? Result.Failure<Guid?>(Error.NotFound(nameof(LoanProvider), providerId.Value))
                : Result.Success<Guid?>(existing.Id);
        }

        var trimmed = lenderName.Trim();
        var byName = await _loanProviderRepository.FindByNameForUserAsync(
            userId,
            trimmed,
            cancellationToken);

        if (byName is not null)
        {
            return Result.Success<Guid?>(byName.Id);
        }

        var provider = new LoanProvider
        {
            UserId = userId,
            Name = trimmed,
            IsActive = true,
        };

        await _loanProviderRepository.AddAsync(provider, cancellationToken);
        return Result.Success<Guid?>(provider.Id);
    }

    private LoanResponse MapDetail(Loan loan)
    {
        var response = _mapper.Map<LoanResponse>(loan);
        ApplyComputedFields(response, loan);

        foreach (var reminder in response.Reminders)
        {
            reminder.LoanName = loan.Name;
            reminder.LoanId = loan.Id;
        }

        return response;
    }

    private LoanListItemResponse MapListItem(Loan loan)
    {
        var item = _mapper.Map<LoanListItemResponse>(loan);
        item.LoanProgressPercent = _calculator.CalculateLoanProgressPercent(
            loan.Principal,
            loan.OutstandingBalance);
        return item;
    }

    private void ApplyComputedFields(LoanResponse response, Loan loan)
    {
        response.TotalPrincipalPaid = _calculator.CalculateTotalPrincipalPaid(
            loan.Principal,
            loan.OutstandingBalance);
        response.TotalInterestPaid = _calculator.CalculateTotalInterestPaid(
            loan.Payments
                .Where(payment => payment.Status == LoanPaymentStatus.Paid)
                .Select(payment => payment.InterestComponent));
        response.LoanProgressPercent = _calculator.CalculateLoanProgressPercent(
            loan.Principal,
            loan.OutstandingBalance);
        response.EmiProgressPercent = _calculator.CalculateEmiProgressPercent(
            loan.TenureMonths,
            loan.RemainingTenureMonths);
        response.RemainingTenureMonths = _calculator.CalculateRemainingTenureMonths(
            loan.TenureMonths,
            loan.RemainingTenureMonths);
    }

    private static void ApplyUpdate(Loan loan, UpdateLoanRequest request, Guid? providerId)
    {
        loan.Name = request.Name.Trim();
        loan.Type = request.Type;
        loan.LenderName = request.LenderName.Trim();
        loan.LoanProviderId = providerId;
        loan.AccountNumber = request.AccountNumber;
        loan.Principal = request.Principal;
        loan.OutstandingBalance = request.OutstandingBalance;
        loan.InterestRate = request.InterestRate;
        loan.InterestType = request.InterestType;
        loan.EmiAmount = request.EmiAmount;
        loan.TenureMonths = request.TenureMonths;
        loan.RemainingTenureMonths = request.RemainingTenureMonths;
        loan.StartDate = request.StartDate;
        loan.EndDate = request.EndDate;
        loan.NextEmiDate = request.NextEmiDate;
        loan.PaymentFrequency = request.PaymentFrequency;
        loan.Status = request.Status;
        loan.LinkedPropertyId = request.LinkedPropertyId;
        loan.CurrencyCode = string.IsNullOrWhiteSpace(request.CurrencyCode)
            ? "INR"
            : request.CurrencyCode.Trim().ToUpperInvariant();
        loan.AutoDebit = request.AutoDebit;
        loan.Notes = request.Notes;
    }

    private static void SyncPrimaryPropertyLink(Loan loan)
    {
        if (!loan.LinkedPropertyId.HasValue)
        {
            return;
        }

        var primary = loan.PropertyLinks.FirstOrDefault(link => link.IsPrimary)
            ?? loan.PropertyLinks.FirstOrDefault();

        if (primary is null)
        {
            loan.PropertyLinks.Add(new LoanPropertyLink
            {
                PropertyId = loan.LinkedPropertyId.Value,
                IsPrimary = true,
            });
            return;
        }

        primary.PropertyId = loan.LinkedPropertyId.Value;
        primary.IsPrimary = true;
    }
}
