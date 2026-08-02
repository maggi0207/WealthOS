using AutoMapper;
using WealthOS.Application.Authentication.DTOs.Responses;
using WealthOS.Domain.Authentication.Entities;

namespace WealthOS.Application.Authentication.Mapping;

public sealed class AuthenticationMappingProfile : Profile
{
    public AuthenticationMappingProfile()
    {
        CreateMap<User, UserProfileResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email ?? string.Empty))
            .ForMember(dest => dest.Roles, opt => opt.Ignore());
    }
}
