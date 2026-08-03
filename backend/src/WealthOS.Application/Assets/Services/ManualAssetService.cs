using WealthOS.Application.Assets.DTOs.Requests;
using WealthOS.Application.Assets.DTOs.Responses;
using WealthOS.Application.Assets.Interfaces;
using WealthOS.Application.Common.Interfaces;
using WealthOS.Application.Common.Models;
using WealthOS.Domain.Assets.Entities;
using WealthOS.Domain.Assets.Enums;
using WealthOS.Domain.Assets.Repositories;
using WealthOS.Domain.Common.Abstractions.Repositories;

namespace WealthOS.Application.Assets.Services;

/// <summary>
/// Orchestrates manual asset CRUD for the authenticated user.
/// </summary>
public sealed class ManualAssetService : IManualAssetService
{
    private readonly IManualAssetRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public ManualAssetService(
        IManualAssetRepository repository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<Result<ManualAssetListResponse>> GetAllAsync(
        int page,
        int pageSize,
        string? search,
        ManualAssetType? type,
        CancellationToken cancellationToken = default)
    {
        var userResult = RequireUserId();
        if (userResult.IsFailure)
        {
            return Result.Failure<ManualAssetListResponse>(userResult.Error!);
        }

        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var (items, totalCount) = await _repository.ListForUserAsync(
            userResult.Value,
            page,
            pageSize,
            search,
            type,
            cancellationToken);

        var totalValue = await _repository.GetTotalCurrentValueAsync(
            userResult.Value,
            cancellationToken);

        return Result.Success(new ManualAssetListResponse
        {
            Items = items.Select(Map).ToList(),
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = totalCount == 0 ? 0 : (int)Math.Ceiling(totalCount / (double)pageSize),
            TotalCurrentValue = totalValue,
            CurrencyCode = items.FirstOrDefault()?.CurrencyCode ?? "INR",
        });
    }

    public async Task<Result<ManualAssetResponse>> GetByIdAsync(
        Guid assetId,
        CancellationToken cancellationToken = default)
    {
        var userResult = RequireUserId();
        if (userResult.IsFailure)
        {
            return Result.Failure<ManualAssetResponse>(userResult.Error!);
        }

        var asset = await _repository.GetByIdForUserAsync(assetId, userResult.Value, cancellationToken);
        if (asset is null)
        {
            return Result.Failure<ManualAssetResponse>(Error.NotFound(nameof(ManualAsset), assetId));
        }

        return Result.Success(Map(asset));
    }

    public async Task<Result<ManualAssetResponse>> CreateAsync(
        CreateManualAssetRequest request,
        CancellationToken cancellationToken = default)
    {
        var userResult = RequireUserId();
        if (userResult.IsFailure)
        {
            return Result.Failure<ManualAssetResponse>(userResult.Error!);
        }

        var asset = new ManualAsset
        {
            UserId = userResult.Value,
            Type = request.Type,
            Name = request.Name.Trim(),
            PurchaseValue = request.PurchaseValue,
            CurrentValue = request.CurrentValue,
            Quantity = request.Quantity,
            Institution = NormalizeOptional(request.Institution),
            PurchaseDate = request.PurchaseDate,
            Notes = NormalizeOptional(request.Notes),
            CurrencyCode = NormalizeCurrency(request.CurrencyCode),
        };

        await _repository.AddAsync(asset, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(Map(asset));
    }

    public async Task<Result<ManualAssetResponse>> UpdateAsync(
        Guid assetId,
        UpdateManualAssetRequest request,
        CancellationToken cancellationToken = default)
    {
        var userResult = RequireUserId();
        if (userResult.IsFailure)
        {
            return Result.Failure<ManualAssetResponse>(userResult.Error!);
        }

        var asset = await _repository.GetByIdForUserAsync(assetId, userResult.Value, cancellationToken);
        if (asset is null)
        {
            return Result.Failure<ManualAssetResponse>(Error.NotFound(nameof(ManualAsset), assetId));
        }

        asset.Type = request.Type;
        asset.Name = request.Name.Trim();
        asset.PurchaseValue = request.PurchaseValue;
        asset.CurrentValue = request.CurrentValue;
        asset.Quantity = request.Quantity;
        asset.Institution = NormalizeOptional(request.Institution);
        asset.PurchaseDate = request.PurchaseDate;
        asset.Notes = NormalizeOptional(request.Notes);
        asset.CurrencyCode = NormalizeCurrency(request.CurrencyCode);

        _repository.Update(asset);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(Map(asset));
    }

    public async Task<Result> DeleteAsync(
        Guid assetId,
        CancellationToken cancellationToken = default)
    {
        var userResult = RequireUserId();
        if (userResult.IsFailure)
        {
            return Result.Failure(userResult.Error!);
        }

        var asset = await _repository.GetByIdForUserAsync(assetId, userResult.Value, cancellationToken);
        if (asset is null)
        {
            return Result.Failure(Error.NotFound(nameof(ManualAsset), assetId));
        }

        asset.IsDeleted = true;
        asset.DeletedAt = DateTime.UtcNow;
        _repository.Update(asset);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    private static ManualAssetResponse Map(ManualAsset asset)
    {
        var gain = asset.CurrentValue - asset.PurchaseValue;
        decimal? gainPct = asset.PurchaseValue > 0
            ? Math.Round(gain / asset.PurchaseValue * 100m, 2)
            : null;

        return new ManualAssetResponse
        {
            Id = asset.Id,
            Type = asset.Type,
            TypeLabel = FormatTypeLabel(asset.Type),
            Name = asset.Name,
            PurchaseValue = asset.PurchaseValue,
            CurrentValue = asset.CurrentValue,
            GainLoss = gain,
            GainLossPercent = gainPct,
            Quantity = asset.Quantity,
            Institution = asset.Institution,
            PurchaseDate = asset.PurchaseDate,
            Notes = asset.Notes,
            CurrencyCode = asset.CurrencyCode,
            CreatedAt = asset.CreatedAt,
            UpdatedAt = asset.UpdatedAt,
        };
    }

    private static string FormatTypeLabel(ManualAssetType type) => type switch
    {
        ManualAssetType.PhysicalGold => "Physical Gold",
        ManualAssetType.Cash => "Cash",
        ManualAssetType.BankBalance => "Bank Balance",
        ManualAssetType.FixedDeposit => "Fixed Deposit",
        ManualAssetType.Vehicle => "Vehicle",
        ManualAssetType.Jewellery => "Jewellery",
        ManualAssetType.Ppf => "PPF",
        ManualAssetType.Epf => "EPF",
        ManualAssetType.Nps => "NPS",
        ManualAssetType.Crypto => "Crypto",
        ManualAssetType.Collectibles => "Collectibles",
        _ => "Other",
    };

    private static string? NormalizeOptional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static string NormalizeCurrency(string? currencyCode) =>
        string.IsNullOrWhiteSpace(currencyCode) ? "INR" : currencyCode.Trim().ToUpperInvariant();

    private Result<Guid> RequireUserId()
    {
        if (!_currentUser.IsAuthenticated || _currentUser.UserId is null)
        {
            return Result.Failure<Guid>(Error.Unauthorized());
        }

        return Result.Success(_currentUser.UserId.Value);
    }
}
