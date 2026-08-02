using FluentValidation;
using WealthOS.Application.AI.DTOs.Requests;
using WealthOS.Application.AI.Queries;

namespace WealthOS.Application.AI.Validators;

public sealed class StartConversationRequestValidator : AbstractValidator<StartConversationRequest>
{
    public StartConversationRequestValidator()
    {
        RuleFor(request => request.Title).MaximumLength(200);
        RuleFor(request => request.PreferredProvider)
            .IsInEnum()
            .When(request => request.PreferredProvider.HasValue);
    }
}

public sealed class SendMessageRequestValidator : AbstractValidator<SendMessageRequest>
{
    public SendMessageRequestValidator()
    {
        RuleFor(request => request.Message).NotEmpty().MaximumLength(8000);
        RuleFor(request => request.PromptTemplateCode).MaximumLength(100);
        RuleFor(request => request.ConversationId)
            .NotEmpty()
            .When(request => request.ConversationId.HasValue);
    }
}

public sealed class SaveMemoryRequestValidator : AbstractValidator<SaveMemoryRequest>
{
    public SaveMemoryRequestValidator()
    {
        RuleFor(request => request.Key).NotEmpty().MaximumLength(200);
        RuleFor(request => request.Content).NotEmpty().MaximumLength(8000);
        RuleFor(request => request.MemoryType).IsInEnum();
        RuleFor(request => request.Importance).InclusiveBetween(0, 1);
        RuleFor(request => request.MetadataJson).MaximumLength(8000);
    }
}

public sealed class GetConversationHistoryQueryValidator : AbstractValidator<GetConversationHistoryQuery>
{
    public GetConversationHistoryQueryValidator()
    {
        RuleFor(query => query.Page).GreaterThanOrEqualTo(1);
        RuleFor(query => query.PageSize).InclusiveBetween(1, 100);
    }
}
