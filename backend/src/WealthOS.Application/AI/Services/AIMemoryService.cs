using WealthOS.Application.AI.DTOs.Requests;
using WealthOS.Application.AI.DTOs.Responses;
using WealthOS.Application.AI.Interfaces;
using WealthOS.Application.Common.Interfaces;
using WealthOS.Application.Common.Models;
using WealthOS.Domain.AI.Entities;
using WealthOS.Domain.AI.Enums;
using WealthOS.Domain.AI.Repositories;
using WealthOS.Domain.Common.Abstractions.Repositories;

namespace WealthOS.Application.AI.Services;

/// <summary>
/// Persists conversation memory, preferences, and important facts for future long-term recall.
/// </summary>
public sealed class AIMemoryService : IAIMemoryService
{
    private readonly IAIMemoryRepository _memoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public AIMemoryService(
        IAIMemoryRepository memoryRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser)
    {
        _memoryRepository = memoryRepository;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<Result<AIMemoryResponse>> SaveAsync(
        SaveMemoryRequest request,
        CancellationToken cancellationToken = default)
    {
        var userResult = RequireUserId();
        if (userResult.IsFailure)
        {
            return Result.Failure<AIMemoryResponse>(userResult.Error!);
        }

        var existing = await _memoryRepository.GetByKeyAsync(
            userResult.Value,
            request.Key,
            cancellationToken);

        if (existing is not null)
        {
            existing.Content = request.Content;
            existing.MemoryType = request.MemoryType;
            existing.MetadataJson = request.MetadataJson;
            existing.Importance = request.Importance;
            existing.ExpiresAt = request.ExpiresAt;
            existing.ConversationId = request.ConversationId;
            _memoryRepository.Update(existing);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success(Map(existing));
        }

        var memory = new AIMemory
        {
            UserId = userResult.Value,
            ConversationId = request.ConversationId,
            MemoryType = request.MemoryType,
            Key = request.Key.Trim(),
            Content = request.Content.Trim(),
            MetadataJson = request.MetadataJson,
            Importance = request.Importance,
            ExpiresAt = request.ExpiresAt,
        };

        await _memoryRepository.AddAsync(memory, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success(Map(memory));
    }

    public async Task<Result<AIMemoryListResponse>> ListAsync(
        AIMemoryType? memoryType,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var userResult = RequireUserId();
        if (userResult.IsFailure)
        {
            return Result.Failure<AIMemoryListResponse>(userResult.Error!);
        }

        var (items, total) = await _memoryRepository.ListForUserAsync(
            userResult.Value,
            memoryType,
            page,
            pageSize,
            cancellationToken);

        return Result.Success(new AIMemoryListResponse
        {
            Items = items.Select(Map).ToList(),
            Page = page,
            PageSize = pageSize,
            TotalCount = total,
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

    private static AIMemoryResponse Map(AIMemory memory) =>
        new()
        {
            Id = memory.Id,
            MemoryType = memory.MemoryType,
            Key = memory.Key,
            Content = memory.Content,
            Importance = memory.Importance,
            CreatedAt = memory.CreatedAt,
        };
}
