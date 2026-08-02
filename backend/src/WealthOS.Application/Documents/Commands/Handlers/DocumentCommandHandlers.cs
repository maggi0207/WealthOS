using WealthOS.Application.Common.Abstractions;
using WealthOS.Application.Common.Models;
using WealthOS.Application.Documents.Commands;
using WealthOS.Application.Documents.DTOs.Responses;
using WealthOS.Application.Documents.Interfaces;

namespace WealthOS.Application.Documents.Commands.Handlers;

public sealed class CreateDocumentCommandHandler
    : ICommandHandler<CreateDocumentCommand, DocumentResponse>
{
    private readonly IDocumentService _documentService;

    public CreateDocumentCommandHandler(IDocumentService documentService)
    {
        _documentService = documentService;
    }

    public Task<Result<DocumentResponse>> HandleAsync(
        CreateDocumentCommand command,
        CancellationToken cancellationToken = default) =>
        _documentService.CreateAsync(command.Request, cancellationToken);
}

public sealed class UpdateDocumentCommandHandler
    : ICommandHandler<UpdateDocumentCommand, DocumentResponse>
{
    private readonly IDocumentService _documentService;

    public UpdateDocumentCommandHandler(IDocumentService documentService)
    {
        _documentService = documentService;
    }

    public Task<Result<DocumentResponse>> HandleAsync(
        UpdateDocumentCommand command,
        CancellationToken cancellationToken = default) =>
        _documentService.UpdateAsync(command.DocumentId, command.Request, cancellationToken);
}

public sealed class DeleteDocumentCommandHandler : ICommandHandler<DeleteDocumentCommand>
{
    private readonly IDocumentService _documentService;

    public DeleteDocumentCommandHandler(IDocumentService documentService)
    {
        _documentService = documentService;
    }

    public Task<Result> HandleAsync(
        DeleteDocumentCommand command,
        CancellationToken cancellationToken = default) =>
        _documentService.DeleteAsync(command.DocumentId, cancellationToken);
}

public sealed class UploadDocumentMetadataCommandHandler
    : ICommandHandler<UploadDocumentMetadataCommand, DocumentResponse>
{
    private readonly IDocumentService _documentService;

    public UploadDocumentMetadataCommandHandler(IDocumentService documentService)
    {
        _documentService = documentService;
    }

    public Task<Result<DocumentResponse>> HandleAsync(
        UploadDocumentMetadataCommand command,
        CancellationToken cancellationToken = default) =>
        _documentService.UploadMetadataAsync(command.DocumentId, command.Request, cancellationToken);
}

public sealed class AddDocumentTagCommandHandler
    : ICommandHandler<AddDocumentTagCommand, DocumentTagResponse>
{
    private readonly IDocumentService _documentService;

    public AddDocumentTagCommandHandler(IDocumentService documentService)
    {
        _documentService = documentService;
    }

    public Task<Result<DocumentTagResponse>> HandleAsync(
        AddDocumentTagCommand command,
        CancellationToken cancellationToken = default) =>
        _documentService.AddTagAsync(command.DocumentId, command.Request, cancellationToken);
}

public sealed class RemoveDocumentTagCommandHandler : ICommandHandler<RemoveDocumentTagCommand>
{
    private readonly IDocumentService _documentService;

    public RemoveDocumentTagCommandHandler(IDocumentService documentService)
    {
        _documentService = documentService;
    }

    public Task<Result> HandleAsync(
        RemoveDocumentTagCommand command,
        CancellationToken cancellationToken = default) =>
        _documentService.RemoveTagAsync(command.DocumentId, command.TagId, cancellationToken);
}

public sealed class CreateDocumentReminderCommandHandler
    : ICommandHandler<CreateDocumentReminderCommand, DocumentReminderResponse>
{
    private readonly IDocumentService _documentService;

    public CreateDocumentReminderCommandHandler(IDocumentService documentService)
    {
        _documentService = documentService;
    }

    public Task<Result<DocumentReminderResponse>> HandleAsync(
        CreateDocumentReminderCommand command,
        CancellationToken cancellationToken = default) =>
        _documentService.CreateReminderAsync(command.DocumentId, command.Request, cancellationToken);
}
