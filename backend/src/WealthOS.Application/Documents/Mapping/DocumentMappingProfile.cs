using AutoMapper;
using WealthOS.Application.Documents.DTOs.Requests;
using WealthOS.Application.Documents.DTOs.Responses;
using WealthOS.Domain.Documents.Entities;

namespace WealthOS.Application.Documents.Mapping;

/// <summary>
/// AutoMapper profile for Documents domain entities and DTOs.
/// </summary>
public sealed class DocumentMappingProfile : Profile
{
    public DocumentMappingProfile()
    {
        CreateMap<CreateDocumentRequest, Document>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.Metadata, opt => opt.Ignore())
            .ForMember(dest => dest.Tags, opt => opt.Ignore())
            .ForMember(dest => dest.Versions, opt => opt.Ignore())
            .ForMember(dest => dest.Links, opt => opt.Ignore())
            .ForMember(dest => dest.Reminders, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title.Trim()))
            .ForMember(dest => dest.Owner, opt => opt.MapFrom(src => src.Owner.Trim()));

        CreateMap<DocumentMetadata, DocumentMetadataResponse>();
        CreateMap<DocumentTag, DocumentTagResponse>();
        CreateMap<DocumentVersion, DocumentVersionResponse>();
        CreateMap<DocumentLink, DocumentLinkResponse>();
        CreateMap<DocumentReminder, DocumentReminderResponse>();

        CreateMap<Document, DocumentResponse>()
            .ForMember(dest => dest.Metadata, opt => opt.Ignore())
            .ForMember(dest => dest.Tags, opt => opt.Ignore())
            .ForMember(dest => dest.Versions, opt => opt.Ignore())
            .ForMember(dest => dest.Links, opt => opt.Ignore())
            .ForMember(dest => dest.Reminders, opt => opt.Ignore());

        CreateMap<Document, DocumentListItemResponse>()
            .ForMember(dest => dest.Tags, opt => opt.Ignore());
    }
}
