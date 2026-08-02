using AutoMapper;
using WealthOS.Application.Common.Interfaces;
using WealthOS.Application.Common.Models;
using WealthOS.Application.Goals.Calculations;
using WealthOS.Application.Goals.DTOs.Requests;
using WealthOS.Application.Goals.DTOs.Responses;
using WealthOS.Application.Goals.Interfaces;
using WealthOS.Domain.Common.Abstractions.Repositories;
using WealthOS.Domain.Goals.Entities;
using WealthOS.Domain.Goals.Enums;
using WealthOS.Domain.Goals.Repositories;
using WealthOS.Domain.Income.Repositories;
using WealthOS.Domain.Investments.Repositories;
using WealthOS.Domain.Loans.Repositories;
using WealthOS.Domain.Properties.Repositories;

namespace WealthOS.Application.Goals.Services;

/// <summary>
/// Orchestrates goal CRUD, contributions, milestones, progress, and dashboard use cases.
/// </summary>
public sealed class GoalService : IGoalService
{
    private readonly IFinancialGoalRepository _goalRepository;
    private readonly IPropertyRepository _propertyRepository;
    private readonly ILoanRepository _loanRepository;
    private readonly IHoldingRepository _holdingRepository;
    private readonly IInvestmentAccountRepository _accountRepository;
    private readonly IIncomeSourceRepository _incomeSourceRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;
    private readonly IGoalCalculationService _calculator;
    private readonly IMapper _mapper;

    public GoalService(
        IFinancialGoalRepository goalRepository,
        IPropertyRepository propertyRepository,
        ILoanRepository loanRepository,
        IHoldingRepository holdingRepository,
        IInvestmentAccountRepository accountRepository,
        IIncomeSourceRepository incomeSourceRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser,
        IGoalCalculationService calculator,
        IMapper mapper)
    {
        _goalRepository = goalRepository;
        _propertyRepository = propertyRepository;
        _loanRepository = loanRepository;
        _holdingRepository = holdingRepository;
        _accountRepository = accountRepository;
        _incomeSourceRepository = incomeSourceRepository;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _calculator = calculator;
        _mapper = mapper;
    }

    public async Task<Result<GoalResponse>> CreateAsync(
        CreateGoalRequest request,
        CancellationToken cancellationToken = default)
    {
        var userResult = RequireUserId();
        if (userResult.IsFailure)
        {
            return Result.Failure<GoalResponse>(userResult.Error!);
        }

        var linkResult = await ValidateLinksAsync(
            userResult.Value,
            request.LinkedPropertyId,
            request.LinkedInvestmentId,
            request.LinkedLoanId,
            request.LinkedIncomeSourceId,
            cancellationToken);
        if (linkResult.IsFailure)
        {
            return Result.Failure<GoalResponse>(linkResult.Error!);
        }

        var goal = _mapper.Map<FinancialGoal>(request);
        goal.UserId = userResult.Value;

        if (goal.CurrentAmount >= goal.TargetAmount && goal.Status == GoalStatus.Active)
        {
            goal.Status = GoalStatus.Completed;
        }

        if (request.Milestones is { Count: > 0 })
        {
            var order = 0;
            foreach (var milestoneRequest in request.Milestones)
            {
                var milestone = new GoalMilestone
                {
                    Label = milestoneRequest.Label.Trim(),
                    TargetPercent = milestoneRequest.TargetPercent,
                    TargetAmount = milestoneRequest.TargetAmount
                        ?? _calculator.RoundMoney(goal.TargetAmount * milestoneRequest.TargetPercent / 100m),
                    ReachedOn = milestoneRequest.ReachedOn,
                    IsCompleted = milestoneRequest.ReachedOn.HasValue,
                    SortOrder = milestoneRequest.SortOrder > 0 ? milestoneRequest.SortOrder : order,
                };
                goal.Milestones.Add(milestone);
                order++;
            }
        }

        SyncMilestonesWithProgress(goal);

        await _goalRepository.AddAsync(goal, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var created = await _goalRepository.GetByIdWithDetailsAsync(
            goal.Id,
            userResult.Value,
            cancellationToken);

        return Result.Success(MapDetail(created!));
    }

    public async Task<Result<GoalResponse>> UpdateAsync(
        Guid goalId,
        UpdateGoalRequest request,
        CancellationToken cancellationToken = default)
    {
        var userResult = RequireUserId();
        if (userResult.IsFailure)
        {
            return Result.Failure<GoalResponse>(userResult.Error!);
        }

        var goal = await _goalRepository.GetByIdWithDetailsAsync(
            goalId,
            userResult.Value,
            cancellationToken);

        if (goal is null)
        {
            return Result.Failure<GoalResponse>(Error.NotFound(nameof(FinancialGoal), goalId));
        }

        var linkResult = await ValidateLinksAsync(
            userResult.Value,
            request.LinkedPropertyId,
            request.LinkedInvestmentId,
            request.LinkedLoanId,
            request.LinkedIncomeSourceId,
            cancellationToken);
        if (linkResult.IsFailure)
        {
            return Result.Failure<GoalResponse>(linkResult.Error!);
        }

        goal.Name = request.Name.Trim();
        goal.Category = request.Category;
        goal.TargetAmount = request.TargetAmount;
        goal.CurrentAmount = request.CurrentAmount;
        goal.TargetDate = request.TargetDate;
        goal.StartedOn = request.StartedOn;
        goal.MonthlyContribution = request.MonthlyContribution;
        goal.Priority = request.Priority;
        goal.Status = request.Status;
        goal.Description = request.Description;
        goal.CurrencyCode = string.IsNullOrWhiteSpace(request.CurrencyCode)
            ? "INR"
            : request.CurrencyCode.Trim().ToUpperInvariant();
        goal.LinkedPropertyId = request.LinkedPropertyId;
        goal.LinkedInvestmentId = request.LinkedInvestmentId;
        goal.LinkedLoanId = request.LinkedLoanId;
        goal.LinkedIncomeSourceId = request.LinkedIncomeSourceId;

        if (goal.CurrentAmount >= goal.TargetAmount && goal.Status == GoalStatus.Active)
        {
            goal.Status = GoalStatus.Completed;
        }

        SyncMilestonesWithProgress(goal);

        _goalRepository.Update(goal);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var updated = await _goalRepository.GetByIdWithDetailsAsync(
            goalId,
            userResult.Value,
            cancellationToken);

        return Result.Success(MapDetail(updated!));
    }

    public async Task<Result> DeleteAsync(Guid goalId, CancellationToken cancellationToken = default)
    {
        var userResult = RequireUserId();
        if (userResult.IsFailure)
        {
            return Result.Failure(userResult.Error!);
        }

        var goal = await _goalRepository.GetByIdForUserAsync(goalId, userResult.Value, cancellationToken);
        if (goal is null)
        {
            return Result.Failure(Error.NotFound(nameof(FinancialGoal), goalId));
        }

        _goalRepository.Remove(goal);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result<GoalContributionResponse>> RecordContributionAsync(
        Guid goalId,
        RecordGoalContributionRequest request,
        CancellationToken cancellationToken = default)
    {
        var userResult = RequireUserId();
        if (userResult.IsFailure)
        {
            return Result.Failure<GoalContributionResponse>(userResult.Error!);
        }

        var goal = await _goalRepository.GetByIdWithDetailsAsync(
            goalId,
            userResult.Value,
            cancellationToken);

        if (goal is null)
        {
            return Result.Failure<GoalContributionResponse>(Error.NotFound(nameof(FinancialGoal), goalId));
        }

        if (goal.Status is GoalStatus.Cancelled)
        {
            return Result.Failure<GoalContributionResponse>(
                Error.Conflict("Cannot record contributions against a cancelled goal."));
        }

        var contribution = new GoalContribution
        {
            GoalId = goal.Id,
            Amount = request.Amount,
            ContributedOn = request.ContributedOn,
            Notes = request.Notes,
            Source = request.Source,
        };

        goal.Contributions.Add(contribution);
        goal.CurrentAmount = _calculator.RoundMoney(goal.CurrentAmount + request.Amount);

        if (goal.CurrentAmount > goal.TargetAmount)
        {
            goal.CurrentAmount = goal.TargetAmount;
        }

        if (goal.CurrentAmount >= goal.TargetAmount)
        {
            goal.Status = GoalStatus.Completed;
        }
        else if (goal.Status == GoalStatus.Completed)
        {
            goal.Status = GoalStatus.Active;
        }

        SyncMilestonesWithProgress(goal);

        _goalRepository.Update(goal);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(_mapper.Map<GoalContributionResponse>(contribution));
    }

    public async Task<Result<GoalMilestoneResponse>> CompleteMilestoneAsync(
        Guid goalId,
        Guid milestoneId,
        CompleteMilestoneRequest request,
        CancellationToken cancellationToken = default)
    {
        var userResult = RequireUserId();
        if (userResult.IsFailure)
        {
            return Result.Failure<GoalMilestoneResponse>(userResult.Error!);
        }

        var goal = await _goalRepository.GetByIdWithDetailsAsync(
            goalId,
            userResult.Value,
            cancellationToken);

        if (goal is null)
        {
            return Result.Failure<GoalMilestoneResponse>(Error.NotFound(nameof(FinancialGoal), goalId));
        }

        var milestone = goal.Milestones.FirstOrDefault(item => item.Id == milestoneId);
        if (milestone is null)
        {
            return Result.Failure<GoalMilestoneResponse>(Error.NotFound(nameof(GoalMilestone), milestoneId));
        }

        if (milestone.IsCompleted)
        {
            return Result.Failure<GoalMilestoneResponse>(
                Error.Conflict("Milestone is already completed."));
        }

        milestone.IsCompleted = true;
        milestone.ReachedOn = request.ReachedOn ?? DateOnly.FromDateTime(DateTime.UtcNow);

        _goalRepository.Update(goal);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(_mapper.Map<GoalMilestoneResponse>(milestone));
    }

    public async Task<Result<GoalResponse>> GetByIdAsync(
        Guid goalId,
        CancellationToken cancellationToken = default)
    {
        var userResult = RequireUserId();
        if (userResult.IsFailure)
        {
            return Result.Failure<GoalResponse>(userResult.Error!);
        }

        var goal = await _goalRepository.GetByIdWithDetailsAsync(
            goalId,
            userResult.Value,
            cancellationToken);

        if (goal is null)
        {
            return Result.Failure<GoalResponse>(Error.NotFound(nameof(FinancialGoal), goalId));
        }

        return Result.Success(MapDetail(goal));
    }

    public async Task<Result<GoalListResponse>> GetAllAsync(
        int page,
        int pageSize,
        string? search,
        GoalStatus? status,
        GoalCategory? category,
        GoalPriority? priority,
        CancellationToken cancellationToken = default)
    {
        var userResult = RequireUserId();
        if (userResult.IsFailure)
        {
            return Result.Failure<GoalListResponse>(userResult.Error!);
        }

        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var (items, totalCount) = await _goalRepository.ListForUserAsync(
            userResult.Value,
            page,
            pageSize,
            search,
            status,
            category,
            priority,
            cancellationToken);

        var asOf = DateOnly.FromDateTime(DateTime.UtcNow);
        var mapped = items.Select(goal =>
        {
            var item = _mapper.Map<GoalListItemResponse>(goal);
            var progress = _calculator.BuildProgress(
                goal.Id,
                goal.Name,
                goal.TargetAmount,
                goal.CurrentAmount,
                goal.MonthlyContribution,
                goal.TargetDate,
                goal.Status,
                asOf,
                goal.CurrencyCode);
            item.CompletionPercent = progress.CompletionPercent;
            item.Trend = progress.Trend;
            return item;
        }).ToList();

        return Result.Success(new GoalListResponse
        {
            Items = mapped,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = totalCount == 0 ? 0 : (int)Math.Ceiling(totalCount / (double)pageSize),
        });
    }

    public async Task<Result<GoalProgressResponse>> GetProgressAsync(
        Guid goalId,
        CancellationToken cancellationToken = default)
    {
        var userResult = RequireUserId();
        if (userResult.IsFailure)
        {
            return Result.Failure<GoalProgressResponse>(userResult.Error!);
        }

        var goal = await _goalRepository.GetByIdForUserAsync(goalId, userResult.Value, cancellationToken);
        if (goal is null)
        {
            return Result.Failure<GoalProgressResponse>(Error.NotFound(nameof(FinancialGoal), goalId));
        }

        var progress = _calculator.BuildProgress(
            goal.Id,
            goal.Name,
            goal.TargetAmount,
            goal.CurrentAmount,
            goal.MonthlyContribution,
            goal.TargetDate,
            goal.Status,
            DateOnly.FromDateTime(DateTime.UtcNow),
            goal.CurrencyCode);

        return Result.Success(new GoalProgressResponse
        {
            GoalId = progress.GoalId,
            GoalName = progress.GoalName,
            TargetAmount = progress.TargetAmount,
            CurrentAmount = progress.CurrentAmount,
            RemainingAmount = progress.RemainingAmount,
            CompletionPercent = progress.CompletionPercent,
            MonthlyContribution = progress.MonthlyContribution,
            MonthlyRequiredContribution = progress.MonthlyRequiredContribution,
            EstimatedCompletionDate = progress.EstimatedCompletionDate,
            TargetDate = progress.TargetDate,
            Trend = progress.Trend,
            MonthsRemaining = progress.MonthsRemaining,
            CurrencyCode = progress.CurrencyCode,
        });
    }

    public async Task<Result<GoalDashboardResponse>> GetDashboardAsync(
        CancellationToken cancellationToken = default)
    {
        var userResult = RequireUserId();
        if (userResult.IsFailure)
        {
            return Result.Failure<GoalDashboardResponse>(userResult.Error!);
        }

        var summary = await _goalRepository.GetDashboardSummaryAsync(
            userResult.Value,
            cancellationToken);

        return Result.Success(new GoalDashboardResponse
        {
            ActiveGoals = summary.ActiveGoals,
            CompletedGoals = summary.CompletedGoals,
            PausedGoals = summary.PausedGoals,
            TotalGoalValue = summary.TotalGoalValue,
            TotalSaved = summary.TotalSaved,
            OverallProgressPercent = summary.OverallProgressPercent,
            MonthlyCommitted = summary.MonthlyCommitted,
            UpcomingMilestones = summary.UpcomingMilestones.Select(item => new GoalUpcomingMilestoneResponse
            {
                MilestoneId = item.MilestoneId,
                GoalId = item.GoalId,
                GoalName = item.GoalName,
                Label = item.Label,
                TargetPercent = item.TargetPercent,
                GoalCompletionPercent = item.GoalCompletionPercent,
            }).ToList(),
            // Extension point: AI / rule-based recommendations (empty in Phase 8).
            Recommendations = Array.Empty<GoalRecommendationResponse>(),
            CurrencyCode = summary.CurrencyCode,
        });
    }

    private GoalResponse MapDetail(FinancialGoal goal)
    {
        var response = _mapper.Map<GoalResponse>(goal);
        var progress = _calculator.BuildProgress(
            goal.Id,
            goal.Name,
            goal.TargetAmount,
            goal.CurrentAmount,
            goal.MonthlyContribution,
            goal.TargetDate,
            goal.Status,
            DateOnly.FromDateTime(DateTime.UtcNow),
            goal.CurrencyCode);

        response.RemainingAmount = progress.RemainingAmount;
        response.CompletionPercent = progress.CompletionPercent;
        response.MonthlyRequiredContribution = progress.MonthlyRequiredContribution;
        response.EstimatedCompletionDate = progress.EstimatedCompletionDate;
        response.Trend = progress.Trend;
        response.Contributions = goal.Contributions
            .OrderByDescending(c => c.ContributedOn)
            .Select(c => _mapper.Map<GoalContributionResponse>(c))
            .ToList();
        response.Milestones = goal.Milestones
            .OrderBy(m => m.SortOrder)
            .ThenBy(m => m.TargetPercent)
            .Select(m => _mapper.Map<GoalMilestoneResponse>(m))
            .ToList();

        return response;
    }

    private static void SyncMilestonesWithProgress(FinancialGoal goal)
    {
        if (goal.TargetAmount <= 0m)
        {
            return;
        }

        var percent = goal.CurrentAmount / goal.TargetAmount * 100m;
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        foreach (var milestone in goal.Milestones.Where(m => !m.IsCompleted && percent >= m.TargetPercent))
        {
            milestone.IsCompleted = true;
            milestone.ReachedOn ??= today;
        }
    }

    private async Task<Result> ValidateLinksAsync(
        Guid userId,
        Guid? linkedPropertyId,
        Guid? linkedInvestmentId,
        Guid? linkedLoanId,
        Guid? linkedIncomeSourceId,
        CancellationToken cancellationToken)
    {
        if (linkedPropertyId.HasValue)
        {
            var exists = await _propertyRepository.ExistsForUserAsync(
                linkedPropertyId.Value,
                userId,
                cancellationToken);
            if (!exists)
            {
                return Result.Failure(Error.NotFound("Property", linkedPropertyId.Value));
            }
        }

        if (linkedLoanId.HasValue)
        {
            var exists = await _loanRepository.ExistsForUserAsync(
                linkedLoanId.Value,
                userId,
                cancellationToken);
            if (!exists)
            {
                return Result.Failure(Error.NotFound("Loan", linkedLoanId.Value));
            }
        }

        if (linkedInvestmentId.HasValue)
        {
            var holding = await _holdingRepository.GetByIdForUserAsync(
                linkedInvestmentId.Value,
                userId,
                cancellationToken);
            var accountExists = holding is null
                && await _accountRepository.ExistsForUserAsync(
                    linkedInvestmentId.Value,
                    userId,
                    cancellationToken);

            if (holding is null && !accountExists)
            {
                return Result.Failure(Error.NotFound("Investment", linkedInvestmentId.Value));
            }
        }

        if (linkedIncomeSourceId.HasValue)
        {
            var source = await _incomeSourceRepository.GetByIdAsync(
                linkedIncomeSourceId.Value,
                cancellationToken);
            if (source is null || source.UserId != userId)
            {
                return Result.Failure(Error.NotFound("IncomeSource", linkedIncomeSourceId.Value));
            }
        }

        return Result.Success();
    }

    private Result<Guid> RequireUserId()
    {
        if (!_currentUser.IsAuthenticated || _currentUser.UserId is null)
        {
            return Result.Failure<Guid>(Error.Unauthorized());
        }

        return Result.Success(_currentUser.UserId.Value);
    }
}
