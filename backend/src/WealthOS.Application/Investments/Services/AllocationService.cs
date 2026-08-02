using WealthOS.Application.Common.Interfaces;
using WealthOS.Application.Common.Models;
using WealthOS.Application.Investments.Calculations;
using WealthOS.Application.Investments.DTOs.Responses;
using WealthOS.Application.Investments.Interfaces;
using WealthOS.Domain.Investments.Enums;
using WealthOS.Domain.Investments.Models;
using WealthOS.Domain.Investments.Repositories;

namespace WealthOS.Application.Investments.Services;

/// <summary>
/// Asset allocation derived from holdings (not persisted).
/// </summary>
public sealed class AllocationService : IAllocationService
{
    private readonly IHoldingRepository _holdingRepository;
    private readonly IInvestmentCalculationService _calculator;
    private readonly ICurrentUserService _currentUser;

    public AllocationService(
        IHoldingRepository holdingRepository,
        IInvestmentCalculationService calculator,
        ICurrentUserService currentUser)
    {
        _holdingRepository = holdingRepository;
        _calculator = calculator;
        _currentUser = currentUser;
    }

    public async Task<Result<AssetAllocationResponse>> GetAllocationAsync(
        Guid? accountId = null,
        CancellationToken cancellationToken = default)
    {
        var userResult = RequireUserId();
        if (userResult.IsFailure)
        {
            return Result.Failure<AssetAllocationResponse>(userResult.Error!);
        }

        var holdings = await _holdingRepository.ListAllForUserAsync(userResult.Value, cancellationToken);
        if (accountId.HasValue)
        {
            holdings = holdings.Where(h => h.AccountId == accountId.Value).ToList();
        }

        var allocation = BuildAllocation(holdings.Select(h => (h.Category, h.CurrentValue)));

        return Result.Success(new AssetAllocationResponse
        {
            TotalValue = allocation.TotalValue,
            Slices = allocation.Slices.Select(s => new AssetAllocationSliceResponse
            {
                Category = s.Category,
                CategoryName = s.CategoryName,
                Value = s.Value,
                WeightPercent = s.WeightPercent,
            }).ToList(),
            CurrencyCode = allocation.CurrencyCode,
        });
    }

    public AssetAllocation BuildAllocation(IEnumerable<(InvestmentCategory Category, decimal Value)> holdings) =>
        _calculator.BuildAllocation(holdings);

    private Result<Guid> RequireUserId()
    {
        if (!_currentUser.IsAuthenticated || _currentUser.UserId is null)
        {
            return Result.Failure<Guid>(Error.Unauthorized());
        }

        return Result.Success(_currentUser.UserId.Value);
    }
}
