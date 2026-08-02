using AutoMapper;
using WealthOS.Application.Dashboard.DTOs.Responses;
using WealthOS.Domain.Dashboard.Models;

namespace WealthOS.Application.Dashboard.Mapping;

/// <summary>
/// AutoMapper profile for dashboard domain models to response DTOs.
/// </summary>
public sealed class DashboardMappingProfile : Profile
{
    public DashboardMappingProfile()
    {
        CreateMap<HealthScoreFactor, HealthScoreFactorResponse>();
        CreateMap<HealthScore, HealthScoreResponse>();
        CreateMap<QuickAction, QuickActionResponse>();
        CreateMap<RecentActivity, RecentActivityResponse>();

        CreateMap<FinancialSummary, NetWorthResponse>();

        CreateMap<DashboardSummary, DashboardResponse>()
            .ForMember(dest => dest.NetWorth, opt => opt.MapFrom(src => src.Financials.NetWorth))
            .ForMember(dest => dest.AssetValue, opt => opt.MapFrom(src => src.Financials.AssetValue))
            .ForMember(dest => dest.LiabilityValue, opt => opt.MapFrom(src => src.Financials.LiabilityValue))
            .ForMember(dest => dest.MonthlyIncome, opt => opt.MapFrom(src => src.Financials.MonthlyIncome))
            .ForMember(dest => dest.MonthlyExpense, opt => opt.MapFrom(src => src.Financials.MonthlyExpense))
            .ForMember(dest => dest.InvestmentValue, opt => opt.MapFrom(src => src.Financials.InvestmentValue))
            .ForMember(dest => dest.PropertyValue, opt => opt.MapFrom(src => src.Financials.PropertyValue))
            .ForMember(dest => dest.LoanBalance, opt => opt.MapFrom(src => src.Financials.LoanBalance))
            .ForMember(dest => dest.ChangePercent, opt => opt.MapFrom(src => src.Financials.ChangePercent))
            .ForMember(dest => dest.CurrencyCode, opt => opt.MapFrom(src => src.Financials.CurrencyCode))
            .ForMember(dest => dest.HealthScore, opt => opt.MapFrom(src => src.HealthScore))
            .ForMember(dest => dest.RecentActivities, opt => opt.MapFrom(src => src.RecentActivities))
            .ForMember(dest => dest.QuickActions, opt => opt.MapFrom(src => src.QuickActions))
            .ForMember(dest => dest.GeneratedAt, opt => opt.MapFrom(src => src.GeneratedAt));
    }
}
