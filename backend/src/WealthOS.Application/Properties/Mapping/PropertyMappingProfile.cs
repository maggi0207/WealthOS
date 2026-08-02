using AutoMapper;
using WealthOS.Application.Properties.DTOs.Requests;
using WealthOS.Application.Properties.DTOs.Responses;
using WealthOS.Domain.Properties.Entities;

namespace WealthOS.Application.Properties.Mapping;

/// <summary>
/// AutoMapper profile for Property domain entities and DTOs.
/// </summary>
public sealed class PropertyMappingProfile : Profile
{
    public PropertyMappingProfile()
    {
        CreateMap<PropertyAddressRequest, PropertyAddress>();
        CreateMap<PropertyOwnerRequest, PropertyOwner>();

        CreateMap<CreatePropertyRequest, Property>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Valuations, opt => opt.Ignore())
            .ForMember(dest => dest.LoanLinks, opt => opt.Ignore())
            .ForMember(dest => dest.DocumentLinks, opt => opt.Ignore())
            .ForMember(dest => dest.Images, opt => opt.Ignore())
            .ForMember(dest => dest.PropertyNotes, opt => opt.Ignore())
            .ForMember(
                dest => dest.Name,
                opt => opt.MapFrom(src => src.Name.Trim()))
            .ForMember(
                dest => dest.CurrencyCode,
                opt => opt.MapFrom(src =>
                    string.IsNullOrWhiteSpace(src.CurrencyCode)
                        ? "INR"
                        : src.CurrencyCode.Trim().ToUpperInvariant()))
            .ForMember(
                dest => dest.Owners,
                opt => opt.MapFrom(src => src.Owners ?? Array.Empty<PropertyOwnerRequest>()));

        CreateMap<PropertyAddress, PropertyAddressResponse>();
        CreateMap<PropertyOwner, PropertyOwnerResponse>();
        CreateMap<PropertyValuation, PropertyValuationResponse>();
        CreateMap<PropertyLoanLink, PropertyLoanLinkResponse>();
        CreateMap<PropertyDocumentLink, PropertyDocumentLinkResponse>();
        CreateMap<PropertyImage, PropertyImageResponse>();
        CreateMap<PropertyNote, PropertyNoteResponse>();

        CreateMap<Property, PropertyResponse>()
            .ForMember(dest => dest.PrimaryOwnerName, opt => opt.Ignore())
            .ForMember(dest => dest.Appreciation, opt => opt.Ignore())
            .ForMember(dest => dest.AppreciationPercent, opt => opt.Ignore())
            .ForMember(dest => dest.PropertyNotes, opt => opt.MapFrom(src => src.PropertyNotes));

        CreateMap<Property, PropertyListItemResponse>()
            .ForMember(dest => dest.PrimaryOwnerName, opt => opt.Ignore())
            .ForMember(dest => dest.City, opt => opt.Ignore())
            .ForMember(dest => dest.Locality, opt => opt.Ignore());
    }
}
