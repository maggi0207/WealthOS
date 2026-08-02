using WealthOS.Application.Common.Abstractions;
using WealthOS.Application.Documents.DTOs.Requests;

namespace WealthOS.Application.Documents.Commands;

/// <summary>
/// Creates a new vault document for the authenticated user.
/// </summary>
public sealed class CreateDocumentCommand : ICommand
{
    public CreateDocumentRequest Request { get; init; } = null!;
}

/// <summary>
/// Updates an existing document owned by the authenticated user.
/// </summary>
public sealed class UpdateDocumentCommand : ICommand
{
    public Guid DocumentId { get; init; }

    public UpdateDocumentRequest Request { get; init; } = null!;
}

/// <summary>
/// Soft-deletes a document owned by the authenticated user.
/// </summary>
public sealed class DeleteDocumentCommand : ICommand
{
    public Guid DocumentId { get; init; }
}

/// <summary>
/// Updates storage metadata placeholders and appends a version record.
/// </summary>
public sealed class UploadDocumentMetadataCommand : ICommand
{
    public Guid DocumentId { get; init; }

    public UploadDocumentMetadataRequest Request { get; init; } = null!;
}

/// <summary>
/// Adds a tag to a document.
/// </summary>
public sealed class AddDocumentTagCommand : ICommand
{
    public Guid DocumentId { get; init; }

    public AddDocumentTagRequest Request { get; init; } = null!;
}

/// <summary>
/// Removes a tag from a document.
/// </summary>
public sealed class RemoveDocumentTagCommand : ICommand
{
    public Guid DocumentId { get; init; }

    public Guid TagId { get; init; }
}

/// <summary>
/// Creates a reminder for a document.
/// </summary>
public sealed class CreateDocumentReminderCommand : ICommand
{
    public Guid DocumentId { get; init; }

    public CreateDocumentReminderRequest Request { get; init; } = null!;
}
