using AutoMapper;
using WealthOS.Application.Goals.DTOs.Requests;
using WealthOS.Application.Goals.DTOs.Responses;
using WealthOS.Domain.Goals.Entities;

namespace WealthOS.Application.Goals.Mapping;

/// <summary>
/// AutoMapper profile for Goals domain entities and DTOs.
/// </summary>
public sealed class GoalMappingProfile : Profile
{
    public GoalMappingProfile()
    {
        CreateMap<CreateGoalRequest, FinancialGoal>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.Contributions, opt => opt.Ignore())
            .ForMember(dest => dest.Milestones, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name.Trim()))
            .ForMember(
                dest => dest.CurrencyCode,
                opt => opt.MapFrom(src =>
                    string.IsNullOrWhiteSpace(src.CurrencyCode)
                        ? "INR"
                        : src.CurrencyCode.Trim().ToUpperInvariant()));

        CreateMap<GoalContribution, GoalContributionResponse>();
        CreateMap<GoalMilestone, GoalMilestoneResponse>();

        CreateMap<FinancialGoal, GoalResponse>()
            .ForMember(dest => dest.RemainingAmount, opt => opt.Ignore())
            .ForMember(dest => dest.CompletionPercent, opt => opt.Ignore())
            .ForMember(dest => dest.MonthlyRequiredContribution, opt => opt.Ignore())
            .ForMember(dest => dest.EstimatedCompletionDate, opt => opt.Ignore())
            .ForMember(dest => dest.Trend, opt => opt.Ignore());

        CreateMap<FinancialGoal, GoalListItemResponse>()
            .ForMember(dest => dest.CompletionPercent, opt => opt.Ignore())
            .ForMember(dest => dest.Trend, opt => opt.Ignore());
    }
}
