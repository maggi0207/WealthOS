using System.Text.Json;
using WealthOS.Application.AI.Interfaces;
using WealthOS.Application.Common.Models;
using WealthOS.Application.Dashboard.Interfaces;
using WealthOS.Application.Dashboard.Providers;
using WealthOS.Application.Documents.Interfaces;
using WealthOS.Application.Goals.Interfaces;
using WealthOS.Application.Notifications.Interfaces;
using WealthOS.Domain.AI.Entities;
using WealthOS.Domain.AI.Repositories;
using WealthOS.Domain.Common.Abstractions.Repositories;

namespace WealthOS.Application.AI.Services;

/// <summary>
/// Builds AI financial context by aggregating module data through application interfaces only.
/// Order: Dashboard → Properties → Loans → Income → Investments → Goals → Documents → Notifications.
/// </summary>
public sealed class AIContextBuilder : IAIContextBuilder
{
    private readonly IDashboardService _dashboardService;
    private readonly IPropertySummaryProvider _propertySummaryProvider;
    private readonly ILoanSummaryProvider _loanSummaryProvider;
    private readonly IIncomeSummaryProvider _incomeSummaryProvider;
    private readonly IInvestmentSummaryProvider _investmentSummaryProvider;
    private readonly IGoalService _goalService;
    private readonly IDocumentSummaryProvider _documentSummaryProvider;
    private readonly IDocumentSearchService _documentSearchService;
    private readonly INotificationService _notificationService;
    private readonly IAIContextRepository _contextRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AIContextBuilder(
        IDashboardService dashboardService,
        IPropertySummaryProvider propertySummaryProvider,
        ILoanSummaryProvider loanSummaryProvider,
        IIncomeSummaryProvider incomeSummaryProvider,
        IInvestmentSummaryProvider investmentSummaryProvider,
        IGoalService goalService,
        IDocumentSummaryProvider documentSummaryProvider,
        IDocumentSearchService documentSearchService,
        INotificationService notificationService,
        IAIContextRepository contextRepository,
        IUnitOfWork unitOfWork)
    {
        _dashboardService = dashboardService;
        _propertySummaryProvider = propertySummaryProvider;
        _loanSummaryProvider = loanSummaryProvider;
        _incomeSummaryProvider = incomeSummaryProvider;
        _investmentSummaryProvider = investmentSummaryProvider;
        _goalService = goalService;
        _documentSummaryProvider = documentSummaryProvider;
        _documentSearchService = documentSearchService;
        _notificationService = notificationService;
        _contextRepository = contextRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<AIContextSnapshot>> BuildAsync(
        Guid userId,
        Guid? conversationId,
        CancellationToken cancellationToken = default)
    {
        var sections = new Dictionary<string, object?>();
        var modules = new List<string>();

        await TryAddAsync("Dashboard", modules, sections, async () =>
        {
            var result = await _dashboardService.GetSummaryAsync(cancellationToken);
            return result.IsSuccess ? result.Value : null;
        });

        await TryAddAsync("Properties", modules, sections, async () =>
            await _propertySummaryProvider.GetSummaryAsync(userId, cancellationToken));

        await TryAddAsync("Loans", modules, sections, async () =>
            await _loanSummaryProvider.GetSummaryAsync(userId, cancellationToken));

        await TryAddAsync("Income", modules, sections, async () =>
            await _incomeSummaryProvider.GetSummaryAsync(userId, cancellationToken));

        await TryAddAsync("Investments", modules, sections, async () =>
            await _investmentSummaryProvider.GetSummaryAsync(userId, cancellationToken));

        await TryAddAsync("Goals", modules, sections, async () =>
        {
            var result = await _goalService.GetDashboardAsync(cancellationToken);
            return result.IsSuccess ? result.Value : null;
        });

        await TryAddAsync("Documents", modules, sections, async () =>
        {
            var summary = await _documentSummaryProvider.GetSummaryAsync(userId, cancellationToken);
            var recent = await _documentSearchService.GetRecentAsync(5, cancellationToken);
            return new
            {
                Summary = summary,
                Recent = recent.IsSuccess ? recent.Value : null,
            };
        });

        await TryAddAsync("Notifications", modules, sections, async () =>
        {
            var result = await _notificationService.GetSummaryAsync(cancellationToken);
            return result.IsSuccess ? result.Value : null;
        });

        var builtAt = DateTime.UtcNow;
        var contextJson = JsonSerializer.Serialize(sections);

        var entity = new AIContext
        {
            UserId = userId,
            ConversationId = conversationId,
            ContextJson = contextJson,
            ModulesIncluded = string.Join(",", modules),
            BuiltAt = builtAt,
        };

        await _contextRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new AIContextSnapshot
        {
            UserId = userId,
            ConversationId = conversationId,
            BuiltAt = builtAt,
            ContextJson = contextJson,
            ModulesIncluded = modules,
            Sections = sections,
        });
    }

    private static async Task TryAddAsync(
        string module,
        List<string> modules,
        Dictionary<string, object?> sections,
        Func<Task<object?>> factory)
    {
        try
        {
            var value = await factory();
            if (value is not null)
            {
                sections[module] = value;
                modules.Add(module);
            }
        }
        catch
        {
            // Context builder is best-effort across modules; skip failures.
        }
    }
}
