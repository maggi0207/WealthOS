using FluentAssertions;
using WealthOS.Application.Documents.DTOs.Requests;
using WealthOS.Application.Documents.Validators;
using WealthOS.Domain.Documents.Enums;

namespace WealthOS.UnitTests.Documents;

public sealed class DocumentValidatorTests
{
    private readonly CreateDocumentRequestValidator _createValidator = new();
    private readonly UpdateDocumentRequestValidator _updateValidator = new();
    private readonly UploadDocumentMetadataRequestValidator _uploadValidator = new();
    private readonly AddDocumentTagRequestValidator _tagValidator = new();
    private readonly CreateDocumentReminderRequestValidator _reminderValidator = new();

    [Fact]
    public void CreateDocument_WhenValid_ShouldSucceed()
    {
        var request = new CreateDocumentRequest
        {
            Title = "Sale deed — Ramana Flats",
            Category = DocumentCategory.Property,
            Owner = "Magesh",
            Status = DocumentStatus.Verified,
            AccessLevel = DocumentAccess.Private,
            OriginalFileName = "sale-deed.pdf",
            ContentType = "application/pdf",
            FileSizeBytes = 1_000_000,
            StorageProvider = DocumentStorageProvider.LocalPlaceholder,
            Tags = ["deed", "adyar"],
        };

        var result = _createValidator.Validate(request);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void CreateDocument_WhenTitleEmpty_ShouldFail()
    {
        var request = new CreateDocumentRequest
        {
            Title = "",
            Owner = "Magesh",
        };

        var result = _createValidator.Validate(request);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(error => error.PropertyName == nameof(CreateDocumentRequest.Title));
    }

    [Fact]
    public void CreateDocument_WhenReferenceModuleWithoutId_ShouldFail()
    {
        var request = new CreateDocumentRequest
        {
            Title = "Linked deed",
            Owner = "Magesh",
            ReferenceModule = DocumentReferenceModule.Property,
            ReferenceId = null,
        };

        var result = _createValidator.Validate(request);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(error => error.PropertyName == nameof(CreateDocumentRequest.ReferenceId));
    }

    [Fact]
    public void CreateDocument_WhenExpiryBeforeIssue_ShouldFail()
    {
        var request = new CreateDocumentRequest
        {
            Title = "Policy",
            Owner = "Magesh",
            IssueDate = new DateOnly(2026, 8, 1),
            ExpiryDate = new DateOnly(2025, 1, 1),
        };

        var result = _createValidator.Validate(request);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void UpdateDocument_WhenOwnerEmpty_ShouldFail()
    {
        var request = new UpdateDocumentRequest
        {
            Title = "Updated",
            Owner = "",
            Category = DocumentCategory.Other,
            Status = DocumentStatus.Pending,
        };

        var result = _updateValidator.Validate(request);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void UploadMetadata_WhenFileSizeZero_ShouldFail()
    {
        var request = new UploadDocumentMetadataRequest
        {
            OriginalFileName = "file.pdf",
            ContentType = "application/pdf",
            FileSizeBytes = 0,
        };

        var result = _uploadValidator.Validate(request);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void AddTag_WhenNameEmpty_ShouldFail()
    {
        var result = _tagValidator.Validate(new AddDocumentTagRequest { Name = "" });
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void CreateReminder_WhenMessageEmpty_ShouldFail()
    {
        var result = _reminderValidator.Validate(new CreateDocumentReminderRequest
        {
            ReminderDate = new DateOnly(2026, 9, 1),
            Message = "",
        });
        result.IsValid.Should().BeFalse();
    }
}
