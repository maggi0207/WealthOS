using AutoMapper;
using WealthOS.Application.Common.Interfaces;
using WealthOS.Application.Common.Models;
using WealthOS.Application.Investments.Calculations;
using WealthOS.Application.Investments.DTOs.Requests;
using WealthOS.Application.Investments.DTOs.Responses;
using WealthOS.Application.Investments.Interfaces;
using WealthOS.Domain.Common.Abstractions.Repositories;
using WealthOS.Domain.Investments.Entities;
using WealthOS.Domain.Investments.Enums;
using WealthOS.Domain.Investments.Repositories;

namespace WealthOS.Application.Investments.Services;

/// <summary>
/// Investment accounts, holdings, transactions, providers, and dashboard summary.
/// </summary>
public sealed class InvestmentService : IInvestmentService
{
    private readonly IInvestmentAccountRepository _accountRepository;
    private readonly IHoldingRepository _holdingRepository;
    private readonly IInvestmentTransactionRepository _transactionRepository;
    private readonly IInvestmentProviderRepository _providerRepository;
    private readonly IAllocationService _allocationService;
    private readonly IInvestmentCalculationService _calculator;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;
    private readonly IMapper _mapper;

    public InvestmentService(
        IInvestmentAccountRepository accountRepository,
        IHoldingRepository holdingRepository,
        IInvestmentTransactionRepository transactionRepository,
        IInvestmentProviderRepository providerRepository,
        IAllocationService allocationService,
        IInvestmentCalculationService calculator,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser,
        IMapper mapper)
    {
        _accountRepository = accountRepository;
        _holdingRepository = holdingRepository;
        _transactionRepository = transactionRepository;
        _providerRepository = providerRepository;
        _allocationService = allocationService;
        _calculator = calculator;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<Result<InvestmentAccountResponse>> CreateAccountAsync(
        CreateInvestmentAccountRequest request,
        CancellationToken cancellationToken = default)
    {
        var userResult = RequireUserId();
        if (userResult.IsFailure)
        {
            return Result.Failure<InvestmentAccountResponse>(userResult.Error!);
        }

        var provider = await _providerRepository.GetByIdAsync(request.ProviderId, cancellationToken);
        if (provider is null)
        {
            return Result.Failure<InvestmentAccountResponse>(Error.NotFound("InvestmentProvider", request.ProviderId));
        }

        var account = _mapper.Map<InvestmentAccount>(request);
        account.UserId = userResult.Value;

        await _accountRepository.AddAsync(account, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(MapAccount(account, provider, 0m, 0m, 0m, 0m, 0));
    }

    public async Task<Result<InvestmentAccountResponse>> UpdateAccountAsync(
        Guid accountId,
        UpdateInvestmentAccountRequest request,
        CancellationToken cancellationToken = default)
    {
        var userResult = RequireUserId();
        if (userResult.IsFailure)
        {
            return Result.Failure<InvestmentAccountResponse>(userResult.Error!);
        }

        var account = await _accountRepository.GetByIdWithProviderAsync(accountId, userResult.Value, cancellationToken);
        if (account is null)
        {
            return Result.Failure<InvestmentAccountResponse>(Error.NotFound("InvestmentAccount", accountId));
        }

        account.Name = request.Name.Trim();
        account.OwnerName = request.OwnerName.Trim();
        account.KindLabel = request.KindLabel.Trim();
        account.Status = request.Status;
        account.CurrencyCode = NormalizeCurrency(request.CurrencyCode);
        account.Notes = request.Notes;
        account.ExternalAccountReference = request.ExternalAccountReference;

        _accountRepository.Update(account);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var totals = await AggregateAccountHoldingsAsync(account.Id, userResult.Value, cancellationToken);
        return Result.Success(MapAccount(
            account,
            account.Provider!,
            totals.Invested,
            totals.Current,
            totals.DayChange,
            totals.DayChangePercent,
            totals.Count));
    }

    public async Task<Result> DeleteAccountAsync(Guid accountId, CancellationToken cancellationToken = default)
    {
        var userResult = RequireUserId();
        if (userResult.IsFailure)
        {
            return Result.Failure(userResult.Error!);
        }

        var account = await _accountRepository.GetByIdForUserAsync(accountId, userResult.Value, cancellationToken);
        if (account is null)
        {
            return Result.Failure(Error.NotFound("InvestmentAccount", accountId));
        }

        account.IsDeleted = true;
        account.DeletedAt = DateTime.UtcNow;
        _accountRepository.Update(account);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result<InvestmentAccountListResponse>> GetAccountsAsync(
        int page,
        int pageSize,
        InvestmentAccountStatus? status,
        Guid? providerId,
        CancellationToken cancellationToken = default)
    {
        var userResult = RequireUserId();
        if (userResult.IsFailure)
        {
            return Result.Failure<InvestmentAccountListResponse>(userResult.Error!);
        }

        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var (items, total) = await _accountRepository.ListForUserAsync(
            userResult.Value,
            page,
            pageSize,
            status,
            providerId,
            cancellationToken);

        var holdings = await _holdingRepository.ListAllForUserAsync(userResult.Value, cancellationToken);
        var byAccount = holdings.GroupBy(h => h.AccountId).ToDictionary(g => g.Key, g => g.ToList());

        var responses = items.Select(account =>
        {
            byAccount.TryGetValue(account.Id, out var accountHoldings);
            accountHoldings ??= [];
            var invested = accountHoldings.Sum(h => h.InvestedAmount);
            var current = accountHoldings.Sum(h => h.CurrentValue);
            var dayChange = accountHoldings.Sum(h => h.DayChange);
            var prior = current - dayChange;
            var dayPct = prior == 0m ? 0m : _calculator.RoundPercent(dayChange / prior * 100m);
            return MapAccount(
                account,
                account.Provider!,
                invested,
                current,
                dayChange,
                dayPct,
                accountHoldings.Count);
        }).ToList();

        return Result.Success(new InvestmentAccountListResponse
        {
            Items = responses,
            Page = page,
            PageSize = pageSize,
            TotalCount = total,
        });
    }

    public async Task<Result<InvestmentAccountResponse>> GetAccountByIdAsync(
        Guid accountId,
        CancellationToken cancellationToken = default)
    {
        var userResult = RequireUserId();
        if (userResult.IsFailure)
        {
            return Result.Failure<InvestmentAccountResponse>(userResult.Error!);
        }

        var account = await _accountRepository.GetByIdWithProviderAsync(accountId, userResult.Value, cancellationToken);
        if (account is null)
        {
            return Result.Failure<InvestmentAccountResponse>(Error.NotFound("InvestmentAccount", accountId));
        }

        var totals = await AggregateAccountHoldingsAsync(account.Id, userResult.Value, cancellationToken);
        return Result.Success(MapAccount(
            account,
            account.Provider!,
            totals.Invested,
            totals.Current,
            totals.DayChange,
            totals.DayChangePercent,
            totals.Count));
    }

    public async Task<Result<HoldingResponse>> AddManualHoldingAsync(
        AddManualHoldingRequest request,
        CancellationToken cancellationToken = default)
    {
        var userResult = RequireUserId();
        if (userResult.IsFailure)
        {
            return Result.Failure<HoldingResponse>(userResult.Error!);
        }

        var account = await _accountRepository.GetByIdForUserAsync(request.AccountId, userResult.Value, cancellationToken);
        if (account is null)
        {
            return Result.Failure<HoldingResponse>(Error.NotFound("InvestmentAccount", request.AccountId));
        }

        var holding = _mapper.Map<Holding>(request);
        holding.UserId = userResult.Value;
        holding.AccountId = account.Id;

        await _holdingRepository.AddAsync(holding, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(MapHolding(holding, account.Name));
    }

    public async Task<Result<HoldingResponse>> UpdateHoldingAsync(
        Guid holdingId,
        UpdateHoldingRequest request,
        CancellationToken cancellationToken = default)
    {
        var userResult = RequireUserId();
        if (userResult.IsFailure)
        {
            return Result.Failure<HoldingResponse>(userResult.Error!);
        }

        var holding = await _holdingRepository.GetByIdForUserAsync(holdingId, userResult.Value, cancellationToken);
        if (holding is null)
        {
            return Result.Failure<HoldingResponse>(Error.NotFound("Holding", holdingId));
        }

        holding.Name = request.Name.Trim();
        holding.Symbol = request.Symbol.Trim().ToUpperInvariant();
        holding.Category = request.Category;
        holding.InvestmentType = request.InvestmentType;
        holding.Quantity = request.Quantity;
        holding.AverageCost = request.AverageCost;
        holding.InvestedAmount = request.InvestedAmount;
        holding.CurrentPrice = request.CurrentPrice;
        holding.CurrentValue = request.CurrentValue;
        holding.DayChange = request.DayChange;
        holding.DayChangePercent = request.DayChangePercent;
        holding.CurrencyCode = NormalizeCurrency(request.CurrencyCode);
        holding.Notes = request.Notes;

        _holdingRepository.Update(holding);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var account = await _accountRepository.GetByIdForUserAsync(holding.AccountId, userResult.Value, cancellationToken);
        return Result.Success(MapHolding(holding, account?.Name ?? string.Empty));
    }

    public async Task<Result> DeleteHoldingAsync(Guid holdingId, CancellationToken cancellationToken = default)
    {
        var userResult = RequireUserId();
        if (userResult.IsFailure)
        {
            return Result.Failure(userResult.Error!);
        }

        var holding = await _holdingRepository.GetByIdForUserAsync(holdingId, userResult.Value, cancellationToken);
        if (holding is null)
        {
            return Result.Failure(Error.NotFound("Holding", holdingId));
        }

        holding.IsDeleted = true;
        holding.DeletedAt = DateTime.UtcNow;
        _holdingRepository.Update(holding);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result<HoldingListResponse>> GetHoldingsAsync(
        int page,
        int pageSize,
        Guid? accountId,
        InvestmentCategory? category,
        string? search,
        CancellationToken cancellationToken = default)
    {
        var userResult = RequireUserId();
        if (userResult.IsFailure)
        {
            return Result.Failure<HoldingListResponse>(userResult.Error!);
        }

        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var (items, total) = await _holdingRepository.ListForUserAsync(
            userResult.Value,
            page,
            pageSize,
            accountId,
            category,
            search,
            cancellationToken);

        var responses = items.Select(h => MapHolding(h, h.Account?.Name ?? string.Empty)).ToList();

        return Result.Success(new HoldingListResponse
        {
            Items = responses,
            Page = page,
            PageSize = pageSize,
            TotalCount = total,
        });
    }

    public async Task<Result<InvestmentTransactionResponse>> RecordTransactionAsync(
        RecordTransactionRequest request,
        CancellationToken cancellationToken = default)
    {
        var userResult = RequireUserId();
        if (userResult.IsFailure)
        {
            return Result.Failure<InvestmentTransactionResponse>(userResult.Error!);
        }

        var account = await _accountRepository.GetByIdForUserAsync(request.AccountId, userResult.Value, cancellationToken);
        if (account is null)
        {
            return Result.Failure<InvestmentTransactionResponse>(Error.NotFound("InvestmentAccount", request.AccountId));
        }

        string? holdingName = null;
        if (request.HoldingId.HasValue)
        {
            var holding = await _holdingRepository.GetByIdForUserAsync(
                request.HoldingId.Value,
                userResult.Value,
                cancellationToken);
            if (holding is null || holding.AccountId != account.Id)
            {
                return Result.Failure<InvestmentTransactionResponse>(
                    Error.NotFound("Holding", request.HoldingId.Value));
            }

            holdingName = holding.Name;
        }

        var transaction = _mapper.Map<InvestmentTransaction>(request);
        transaction.UserId = userResult.Value;

        await _transactionRepository.AddAsync(transaction, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(MapTransaction(transaction, holdingName));
    }

    public async Task<Result<InvestmentTransactionListResponse>> GetTransactionsAsync(
        int page,
        int pageSize,
        Guid? accountId,
        Guid? holdingId,
        InvestmentTransactionType? type,
        CancellationToken cancellationToken = default)
    {
        var userResult = RequireUserId();
        if (userResult.IsFailure)
        {
            return Result.Failure<InvestmentTransactionListResponse>(userResult.Error!);
        }

        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var (items, total) = await _transactionRepository.ListForUserAsync(
            userResult.Value,
            page,
            pageSize,
            accountId,
            holdingId,
            type,
            cancellationToken);

        return Result.Success(new InvestmentTransactionListResponse
        {
            Items = items.Select(t => MapTransaction(t, t.Holding?.Name)).ToList(),
            Page = page,
            PageSize = pageSize,
            TotalCount = total,
        });
    }

    public async Task<Result<InvestmentProviderListResponse>> GetProvidersAsync(
        CancellationToken cancellationToken = default)
    {
        var userResult = RequireUserId();
        if (userResult.IsFailure)
        {
            return Result.Failure<InvestmentProviderListResponse>(userResult.Error!);
        }

        var providers = await _providerRepository.ListAllAsync(cancellationToken);
        var items = _mapper.Map<IReadOnlyList<InvestmentProviderResponse>>(providers);

        return Result.Success(new InvestmentProviderListResponse
        {
            Items = items,
            TotalCount = items.Count,
        });
    }

    public async Task<Result<InvestmentDashboardResponse>> GetDashboardSummaryAsync(
        CancellationToken cancellationToken = default)
    {
        var userResult = RequireUserId();
        if (userResult.IsFailure)
        {
            return Result.Failure<InvestmentDashboardResponse>(userResult.Error!);
        }

        var holdings = await _holdingRepository.ListAllForUserAsync(userResult.Value, cancellationToken);
        var accountCount = await _accountRepository.CountForUserAsync(userResult.Value, cancellationToken);

        var invested = holdings.Sum(h => h.InvestedAmount);
        var current = holdings.Sum(h => h.CurrentValue);
        var dayChange = holdings.Sum(h => h.DayChange);
        var summary = _calculator.BuildSummary(
            invested,
            current,
            dayChange,
            accountCount,
            holdings.Count,
            holdings.OrderByDescending(h => h.CurrentValue).FirstOrDefault()?.Name,
            holdings.OrderByDescending(h => h.CurrentValue).FirstOrDefault()?.CurrentValue);

        var allocationResult = await _allocationService.GetAllocationAsync(null, cancellationToken);
        var allocation = allocationResult.IsSuccess
            ? allocationResult.Value
            : new AssetAllocationResponse();

        return Result.Success(new InvestmentDashboardResponse
        {
            PortfolioValue = summary.PortfolioValue,
            TodaysGain = summary.TodaysGain,
            TodaysGainPercent = summary.TodaysGainPercent,
            TotalReturn = summary.TotalReturn,
            AbsoluteReturnPercent = summary.AbsoluteReturnPercent,
            AccountCount = summary.AccountCount,
            HoldingCount = summary.HoldingCount,
            LargestHoldingName = summary.LargestHoldingName,
            LargestHoldingValue = summary.LargestHoldingValue,
            Allocation = allocation,
            CurrencyCode = summary.CurrencyCode,
        });
    }

    private async Task<(decimal Invested, decimal Current, decimal DayChange, decimal DayChangePercent, int Count)>
        AggregateAccountHoldingsAsync(Guid accountId, Guid userId, CancellationToken cancellationToken)
    {
        var holdings = await _holdingRepository.ListAllForUserAsync(userId, cancellationToken);
        var accountHoldings = holdings.Where(h => h.AccountId == accountId).ToList();
        var invested = accountHoldings.Sum(h => h.InvestedAmount);
        var current = accountHoldings.Sum(h => h.CurrentValue);
        var dayChange = accountHoldings.Sum(h => h.DayChange);
        var prior = current - dayChange;
        var dayPct = prior == 0m ? 0m : _calculator.RoundPercent(dayChange / prior * 100m);
        return (invested, current, dayChange, dayPct, accountHoldings.Count);
    }

    private HoldingResponse MapHolding(Holding holding, string accountName)
    {
        var overallGain = _calculator.RoundMoney(holding.CurrentValue - holding.InvestedAmount);
        var absoluteReturn = holding.InvestedAmount == 0m
            ? 0m
            : _calculator.RoundPercent(overallGain / holding.InvestedAmount * 100m);

        return new HoldingResponse
        {
            Id = holding.Id,
            AccountId = holding.AccountId,
            AccountName = accountName,
            Name = holding.Name,
            Symbol = holding.Symbol,
            Category = holding.Category,
            InvestmentType = holding.InvestmentType,
            Quantity = holding.Quantity,
            AverageCost = holding.AverageCost,
            InvestedAmount = holding.InvestedAmount,
            CurrentPrice = holding.CurrentPrice,
            CurrentValue = holding.CurrentValue,
            DayChange = holding.DayChange,
            DayChangePercent = holding.DayChangePercent,
            OverallGain = overallGain,
            AbsoluteReturnPercent = absoluteReturn,
            CurrencyCode = holding.CurrencyCode,
            Notes = holding.Notes,
        };
    }

    private static InvestmentAccountResponse MapAccount(
        InvestmentAccount account,
        InvestmentProvider provider,
        decimal invested,
        decimal current,
        decimal dayChange,
        decimal dayChangePercent,
        int holdingsCount) =>
        new()
        {
            Id = account.Id,
            ProviderId = account.ProviderId,
            ProviderName = provider.Name,
            ProviderKind = provider.Kind,
            Name = account.Name,
            OwnerName = account.OwnerName,
            KindLabel = account.KindLabel,
            Status = account.Status,
            LastSyncedAt = account.LastSyncedAt,
            CurrentValue = current,
            InvestedAmount = invested,
            DayChange = dayChange,
            DayChangePercent = dayChangePercent,
            HoldingsCount = holdingsCount,
            CurrencyCode = account.CurrencyCode,
            Notes = account.Notes,
        };

    private static InvestmentTransactionResponse MapTransaction(
        InvestmentTransaction transaction,
        string? holdingName) =>
        new()
        {
            Id = transaction.Id,
            AccountId = transaction.AccountId,
            HoldingId = transaction.HoldingId,
            HoldingName = holdingName,
            TransactionType = transaction.TransactionType,
            Quantity = transaction.Quantity,
            Price = transaction.Price,
            Amount = transaction.Amount,
            Fees = transaction.Fees,
            TransactionDate = transaction.TransactionDate,
            CurrencyCode = transaction.CurrencyCode,
            Notes = transaction.Notes,
            ExternalReference = transaction.ExternalReference,
        };

    private Result<Guid> RequireUserId()
    {
        if (!_currentUser.IsAuthenticated || _currentUser.UserId is null)
        {
            return Result.Failure<Guid>(Error.Unauthorized());
        }

        return Result.Success(_currentUser.UserId.Value);
    }

    private static string NormalizeCurrency(string? code) =>
        string.IsNullOrWhiteSpace(code) ? "INR" : code.Trim().ToUpperInvariant();
}
