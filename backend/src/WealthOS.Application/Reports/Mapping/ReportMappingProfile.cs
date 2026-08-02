using AutoMapper;
using WealthOS.Application.Reports.DTOs.Responses;
using WealthOS.Domain.Reports.Entities;
using WealthOS.Domain.Reports.Models;

namespace WealthOS.Application.Reports.Mapping;

/// <summary>AutoMapper profile for Reports &amp; Analytics entities and DTOs.</summary>
public sealed class ReportMappingProfile : Profile
{
    public ReportMappingProfile()
    {
        CreateMap<ReportSnapshot, ReportSnapshotResponse>()
            .ForMember(dest => dest.Filters, opt => opt.Ignore());

        CreateMap<ReportExport, ReportExportResponse>()
            .ForMember(
                dest => dest.SupportedFormats,
                opt => opt.MapFrom(_ => new[] { "Csv", "Excel", "Pdf", "Json" }));

        CreateMap<FinancialHealthFactor, FinancialHealthFactorResponse>();
        CreateMap<TrendPoint, ReportTrendPointResponse>();
        CreateMap<ReportFilter, ReportFilterResponse>();
        CreateMap<AnalyticsSummary, AnalyticsSummaryResponse>()
            .ForMember(dest => dest.ReportType, opt => opt.Ignore())
            .ForMember(dest => dest.Title, opt => opt.Ignore())
            .ForMember(dest => dest.Filters, opt => opt.Ignore())
            .ForMember(dest => dest.DataSources, opt => opt.Ignore());
        CreateMap<FinancialHealth, FinancialHealthResponse>()
            .ForMember(dest => dest.ReportType, opt => opt.Ignore())
            .ForMember(dest => dest.Title, opt => opt.Ignore())
            .ForMember(dest => dest.GeneratedAt, opt => opt.MapFrom(src => src.CalculatedAt))
            .ForMember(dest => dest.GradeLabel, opt => opt.Ignore())
            .ForMember(dest => dest.Filters, opt => opt.Ignore())
            .ForMember(dest => dest.DataSources, opt => opt.Ignore())
            .ForMember(dest => dest.CurrencyCode, opt => opt.Ignore());
    }
}
