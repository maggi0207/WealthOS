using AutoMapper;
using WealthOS.Application.Investments.DTOs.Requests;
using WealthOS.Application.Investments.DTOs.Responses;
using WealthOS.Domain.Investments.Entities;

namespace WealthOS.Application.Investments.Mapping;

/// <summary>
/// AutoMapper profile for Investments entities and DTOs.
/// </summary>
public sealed class InvestmentMappingProfile : Profile
{
    public InvestmentMappingProfile()
    {
        CreateMap<InvestmentProvider, InvestmentProviderResponse>();

        CreateMap<CreateInvestmentAccountRequest, InvestmentAccount>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.UserId, o => o.Ignore())
            .ForMember(d => d.Provider, o => o.Ignore())
            .ForMember(d => d.Holdings, o => o.Ignore())
            .ForMember(d => d.Snapshots, o => o.Ignore())
            .ForMember(d => d.Transactions, o => o.Ignore())
            .ForMember(d => d.LastSyncedAt, o => o.Ignore())
            .ForMember(d => d.CreatedAt, o => o.Ignore())
            .ForMember(d => d.CreatedBy, o => o.Ignore())
            .ForMember(d => d.UpdatedAt, o => o.Ignore())
            .ForMember(d => d.UpdatedBy, o => o.Ignore())
            .ForMember(d => d.IsDeleted, o => o.Ignore())
            .ForMember(d => d.DeletedAt, o => o.Ignore())
            .ForMember(d => d.Name, o => o.MapFrom(s => s.Name.Trim()))
            .ForMember(d => d.OwnerName, o => o.MapFrom(s => s.OwnerName.Trim()))
            .ForMember(d => d.KindLabel, o => o.MapFrom(s => s.KindLabel.Trim()))
            .ForMember(d => d.CurrencyCode, o => o.MapFrom(s => NormalizeCurrency(s.CurrencyCode)));

        CreateMap<AddManualHoldingRequest, Holding>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.UserId, o => o.Ignore())
            .ForMember(d => d.Account, o => o.Ignore())
            .ForMember(d => d.Transactions, o => o.Ignore())
            .ForMember(d => d.Dividends, o => o.Ignore())
            .ForMember(d => d.CorporateActions, o => o.Ignore())
            .ForMember(d => d.CreatedAt, o => o.Ignore())
            .ForMember(d => d.CreatedBy, o => o.Ignore())
            .ForMember(d => d.UpdatedAt, o => o.Ignore())
            .ForMember(d => d.UpdatedBy, o => o.Ignore())
            .ForMember(d => d.IsDeleted, o => o.Ignore())
            .ForMember(d => d.DeletedAt, o => o.Ignore())
            .ForMember(d => d.Name, o => o.MapFrom(s => s.Name.Trim()))
            .ForMember(d => d.Symbol, o => o.MapFrom(s => s.Symbol.Trim().ToUpperInvariant()))
            .ForMember(d => d.CurrencyCode, o => o.MapFrom(s => NormalizeCurrency(s.CurrencyCode)));

        CreateMap<RecordTransactionRequest, InvestmentTransaction>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.UserId, o => o.Ignore())
            .ForMember(d => d.Account, o => o.Ignore())
            .ForMember(d => d.Holding, o => o.Ignore())
            .ForMember(d => d.CreatedAt, o => o.Ignore())
            .ForMember(d => d.CreatedBy, o => o.Ignore())
            .ForMember(d => d.UpdatedAt, o => o.Ignore())
            .ForMember(d => d.UpdatedBy, o => o.Ignore())
            .ForMember(d => d.IsDeleted, o => o.Ignore())
            .ForMember(d => d.DeletedAt, o => o.Ignore())
            .ForMember(d => d.CurrencyCode, o => o.MapFrom(s => NormalizeCurrency(s.CurrencyCode)));
    }

    private static string NormalizeCurrency(string? code) =>
        string.IsNullOrWhiteSpace(code) ? "INR" : code.Trim().ToUpperInvariant();
}
