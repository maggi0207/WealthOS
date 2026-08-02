using WealthOS.Application.AI.DTOs.Responses;
using WealthOS.Application.AI.Interfaces;
using WealthOS.Application.Common.Models;
using WealthOS.Domain.AI.Repositories;

namespace WealthOS.Application.AI.Services;

/// <summary>
/// Resolves and renders reusable prompt templates with simple {{variable}} substitution.
/// </summary>
public sealed class AIPromptService : IAIPromptService
{
    private readonly IPromptTemplateRepository _promptTemplateRepository;

    public AIPromptService(IPromptTemplateRepository promptTemplateRepository) =>
        _promptTemplateRepository = promptTemplateRepository;

    public async Task<Result<PromptTemplateResponse>> GetByCodeAsync(
        string code,
        CancellationToken cancellationToken = default)
    {
        var template = await _promptTemplateRepository.GetByCodeAsync(code, cancellationToken);
        if (template is null || !template.IsActive)
        {
            return Result.Failure<PromptTemplateResponse>(
                Error.NotFound(nameof(PromptTemplateResponse), code));
        }

        return Result.Success(new PromptTemplateResponse
        {
            Id = template.Id,
            Code = template.Code,
            Name = template.Name,
            Category = template.Category,
            SystemPrompt = template.SystemPrompt,
            UserPromptTemplate = template.UserPromptTemplate,
            Version = template.Version,
        });
    }

    public async Task<Result<string>> RenderAsync(
        string code,
        IReadOnlyDictionary<string, string> variables,
        CancellationToken cancellationToken = default)
    {
        var templateResult = await GetByCodeAsync(code, cancellationToken);
        if (templateResult.IsFailure)
        {
            return Result.Failure<string>(templateResult.Error!);
        }

        var rendered = templateResult.Value.UserPromptTemplate;
        foreach (var (key, value) in variables)
        {
            rendered = rendered.Replace($"{{{{{key}}}}}", value, StringComparison.OrdinalIgnoreCase);
        }

        return Result.Success(rendered);
    }
}
