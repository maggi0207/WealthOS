using Microsoft.Extensions.DependencyInjection;
using WealthOS.Application.Common.Abstractions;
using WealthOS.Application.Documents.Commands;
using WealthOS.Application.Documents.Commands.Handlers;
using WealthOS.Application.Documents.DTOs.Responses;
using WealthOS.Application.Documents.Interfaces;
using WealthOS.Application.Documents.Queries;
using WealthOS.Application.Documents.Queries.Handlers;
using WealthOS.Application.Documents.Services;

namespace WealthOS.Application.Documents;

/// <summary>
/// Registers Documents application services and CQRS handlers.
/// </summary>
public static class DocumentServiceCollectionExtensions
{
    public static IServiceCollection AddDocumentsApplication(this IServiceCollection services)
    {
        services.AddScoped<IDocumentService, DocumentService>();
        services.AddScoped<IDocumentSearchService, DocumentSearchService>();

        services.AddScoped<ICommandHandler<CreateDocumentCommand, DocumentResponse>, CreateDocumentCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateDocumentCommand, DocumentResponse>, UpdateDocumentCommandHandler>();
        services.AddScoped<ICommandHandler<DeleteDocumentCommand>, DeleteDocumentCommandHandler>();
        services.AddScoped<
            ICommandHandler<UploadDocumentMetadataCommand, DocumentResponse>,
            UploadDocumentMetadataCommandHandler>();
        services.AddScoped<
            ICommandHandler<AddDocumentTagCommand, DocumentTagResponse>,
            AddDocumentTagCommandHandler>();
        services.AddScoped<ICommandHandler<RemoveDocumentTagCommand>, RemoveDocumentTagCommandHandler>();
        services.AddScoped<
            ICommandHandler<CreateDocumentReminderCommand, DocumentReminderResponse>,
            CreateDocumentReminderCommandHandler>();

        services.AddScoped<IQueryHandler<GetDocumentsQuery, DocumentListResponse>, GetDocumentsQueryHandler>();
        services.AddScoped<IQueryHandler<GetDocumentByIdQuery, DocumentResponse>, GetDocumentByIdQueryHandler>();
        services.AddScoped<
            IQueryHandler<SearchDocumentsQuery, DocumentListResponse>,
            SearchDocumentsQueryHandler>();
        services.AddScoped<
            IQueryHandler<GetRecentDocumentsQuery, DocumentListResponse>,
            GetRecentDocumentsQueryHandler>();
        services.AddScoped<
            IQueryHandler<GetExpiredDocumentsQuery, DocumentListResponse>,
            GetExpiredDocumentsQueryHandler>();

        return services;
    }
}
