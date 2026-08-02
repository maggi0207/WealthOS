using FluentValidation;
using WealthOS.Application.Documents.DTOs.Requests;
using WealthOS.Application.Documents.Queries;
using WealthOS.Domain.Documents.Enums;

namespace WealthOS.Application.Documents.Validators;

public sealed class CreateDocumentRequestValidator : AbstractValidator<CreateDocumentRequest>
{
    public CreateDocumentRequestValidator()
    {
        RuleFor(request => request.Title).NotEmpty().MaximumLength(300);
        RuleFor(request => request.Description).MaximumLength(4000);
        RuleFor(request => request.Category).IsInEnum();
        RuleFor(request => request.Owner).NotEmpty().MaximumLength(200);
        RuleFor(request => request.Status).IsInEnum();
        RuleFor(request => request.AccessLevel).IsInEnum();
        RuleFor(request => request.ReferenceModule).IsInEnum();
        RuleFor(request => request.Notes).MaximumLength(4000);
        RuleFor(request => request.OriginalFileName).MaximumLength(500);
        RuleFor(request => request.ContentType).MaximumLength(128);
        RuleFor(request => request.FileSizeBytes).GreaterThanOrEqualTo(0);
        RuleFor(request => request.StorageProvider).IsInEnum();
        RuleFor(request => request.StoragePath).MaximumLength(1000);

        RuleFor(request => request.ExpiryDate)
            .GreaterThanOrEqualTo(request => request.IssueDate!.Value)
            .When(request => request.IssueDate.HasValue && request.ExpiryDate.HasValue)
            .WithMessage("Expiry date must be on or after the issue date.");

        RuleFor(request => request.ReferenceId)
            .NotEmpty()
            .When(request => request.ReferenceModule != DocumentReferenceModule.None);

        RuleForEach(request => request.Tags)
            .NotEmpty()
            .MaximumLength(64)
            .When(request => request.Tags is not null);

        RuleForEach(request => request.Links)
            .ChildRules(link =>
            {
                link.RuleFor(item => item.ReferenceModule)
                    .IsInEnum()
                    .Must(module => module != DocumentReferenceModule.None);
                link.RuleFor(item => item.ReferenceId).NotEmpty();
                link.RuleFor(item => item.Notes).MaximumLength(1000);
            })
            .When(request => request.Links is not null);

        When(request => request.Metadata is not null, () =>
        {
            RuleFor(request => request.Metadata!.DocumentNumber).MaximumLength(128);
            RuleFor(request => request.Metadata!.IssuedBy).MaximumLength(200);
            RuleFor(request => request.Metadata!.IssuerCountry).MaximumLength(64);
            RuleFor(request => request.Metadata!.Checksum).MaximumLength(128);
            RuleFor(request => request.Metadata!.PageCount).GreaterThan(0)
                .When(request => request.Metadata!.PageCount.HasValue);
            RuleFor(request => request.Metadata!.CustomAttributesJson).MaximumLength(8000);
        });
    }
}

public sealed class UpdateDocumentRequestValidator : AbstractValidator<UpdateDocumentRequest>
{
    public UpdateDocumentRequestValidator()
    {
        RuleFor(request => request.Title).NotEmpty().MaximumLength(300);
        RuleFor(request => request.Description).MaximumLength(4000);
        RuleFor(request => request.Category).IsInEnum();
        RuleFor(request => request.Owner).NotEmpty().MaximumLength(200);
        RuleFor(request => request.Status).IsInEnum();
        RuleFor(request => request.AccessLevel).IsInEnum();
        RuleFor(request => request.ReferenceModule).IsInEnum();
        RuleFor(request => request.Notes).MaximumLength(4000);

        RuleFor(request => request.ExpiryDate)
            .GreaterThanOrEqualTo(request => request.IssueDate!.Value)
            .When(request => request.IssueDate.HasValue && request.ExpiryDate.HasValue)
            .WithMessage("Expiry date must be on or after the issue date.");

        RuleFor(request => request.ReferenceId)
            .NotEmpty()
            .When(request => request.ReferenceModule != DocumentReferenceModule.None);
    }
}

public sealed class UploadDocumentMetadataRequestValidator
    : AbstractValidator<UploadDocumentMetadataRequest>
{
    public UploadDocumentMetadataRequestValidator()
    {
        RuleFor(request => request.OriginalFileName).NotEmpty().MaximumLength(500);
        RuleFor(request => request.ContentType).NotEmpty().MaximumLength(128);
        RuleFor(request => request.FileSizeBytes).GreaterThan(0);
        RuleFor(request => request.StorageProvider).IsInEnum();
        RuleFor(request => request.StoragePath).MaximumLength(1000);
        RuleFor(request => request.VersionNotes).MaximumLength(1000);

        When(request => request.Metadata is not null, () =>
        {
            RuleFor(request => request.Metadata!.DocumentNumber).MaximumLength(128);
            RuleFor(request => request.Metadata!.IssuedBy).MaximumLength(200);
            RuleFor(request => request.Metadata!.IssuerCountry).MaximumLength(64);
            RuleFor(request => request.Metadata!.Checksum).MaximumLength(128);
            RuleFor(request => request.Metadata!.PageCount).GreaterThan(0)
                .When(request => request.Metadata!.PageCount.HasValue);
            RuleFor(request => request.Metadata!.CustomAttributesJson).MaximumLength(8000);
        });
    }
}

public sealed class AddDocumentTagRequestValidator : AbstractValidator<AddDocumentTagRequest>
{
    public AddDocumentTagRequestValidator()
    {
        RuleFor(request => request.Name).NotEmpty().MaximumLength(64);
    }
}

public sealed class CreateDocumentReminderRequestValidator
    : AbstractValidator<CreateDocumentReminderRequest>
{
    public CreateDocumentReminderRequestValidator()
    {
        RuleFor(request => request.Message).NotEmpty().MaximumLength(500);
        RuleFor(request => request.Notes).MaximumLength(1000);
    }
}

public sealed class GetDocumentsQueryValidator : AbstractValidator<GetDocumentsQuery>
{
    public GetDocumentsQueryValidator()
    {
        RuleFor(query => query.Page).GreaterThanOrEqualTo(1);
        RuleFor(query => query.PageSize).InclusiveBetween(1, 100);
        RuleFor(query => query.Search).MaximumLength(200);
        RuleFor(query => query.Category).IsInEnum().When(query => query.Category.HasValue);
        RuleFor(query => query.Status).IsInEnum().When(query => query.Status.HasValue);
    }
}

public sealed class SearchDocumentsQueryValidator : AbstractValidator<SearchDocumentsQuery>
{
    public SearchDocumentsQueryValidator()
    {
        RuleFor(query => query.Page).GreaterThanOrEqualTo(1);
        RuleFor(query => query.PageSize).InclusiveBetween(1, 100);
        RuleFor(query => query.Title).MaximumLength(300);
        RuleFor(query => query.Tag).MaximumLength(64);
        RuleFor(query => query.Owner).MaximumLength(200);
        RuleFor(query => query.FreeText).MaximumLength(200);
        RuleFor(query => query.Category).IsInEnum().When(query => query.Category.HasValue);
        RuleFor(query => query.Status).IsInEnum().When(query => query.Status.HasValue);
        RuleFor(query => query.ReferenceModule).IsInEnum().When(query => query.ReferenceModule.HasValue);
    }
}

public sealed class GetRecentDocumentsQueryValidator : AbstractValidator<GetRecentDocumentsQuery>
{
    public GetRecentDocumentsQueryValidator()
    {
        RuleFor(query => query.Take).InclusiveBetween(1, 50);
    }
}

public sealed class GetExpiredDocumentsQueryValidator : AbstractValidator<GetExpiredDocumentsQuery>
{
    public GetExpiredDocumentsQueryValidator()
    {
        RuleFor(query => query.Take).InclusiveBetween(1, 100);
    }
}
