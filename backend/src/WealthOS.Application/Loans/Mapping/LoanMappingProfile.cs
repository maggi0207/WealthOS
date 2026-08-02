using AutoMapper;
using WealthOS.Application.Loans.DTOs.Requests;
using WealthOS.Application.Loans.DTOs.Responses;
using WealthOS.Domain.Loans.Entities;

namespace WealthOS.Application.Loans.Mapping;

/// <summary>
/// AutoMapper profile for Loan domain entities and DTOs.
/// </summary>
public sealed class LoanMappingProfile : Profile
{
    public LoanMappingProfile()
    {
        CreateMap<CreateLoanRequest, Loan>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.LoanProvider, opt => opt.Ignore())
            .ForMember(dest => dest.LinkedProperty, opt => opt.Ignore())
            .ForMember(dest => dest.Payments, opt => opt.Ignore())
            .ForMember(dest => dest.Schedules, opt => opt.Ignore())
            .ForMember(dest => dest.Reminders, opt => opt.Ignore())
            .ForMember(dest => dest.InterestRates, opt => opt.Ignore())
            .ForMember(dest => dest.DocumentLinks, opt => opt.Ignore())
            .ForMember(dest => dest.PropertyLinks, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
            .ForMember(
                dest => dest.Name,
                opt => opt.MapFrom(src => src.Name.Trim()))
            .ForMember(
                dest => dest.LenderName,
                opt => opt.MapFrom(src => src.LenderName.Trim()))
            .ForMember(
                dest => dest.CurrencyCode,
                opt => opt.MapFrom(src =>
                    string.IsNullOrWhiteSpace(src.CurrencyCode)
                        ? "INR"
                        : src.CurrencyCode.Trim().ToUpperInvariant()));

        CreateMap<LoanProvider, LoanProviderResponse>();
        CreateMap<LoanPayment, LoanPaymentResponse>();
        CreateMap<LoanInterestRate, LoanInterestRateResponse>();
        CreateMap<LoanDocumentLink, LoanDocumentLinkResponse>();
        CreateMap<LoanPropertyLink, LoanPropertyLinkResponse>();

        CreateMap<LoanReminder, LoanReminderResponse>()
            .ForMember(dest => dest.LoanName, opt => opt.Ignore());

        CreateMap<Loan, LoanResponse>()
            .ForMember(dest => dest.TotalPrincipalPaid, opt => opt.Ignore())
            .ForMember(dest => dest.TotalInterestPaid, opt => opt.Ignore())
            .ForMember(dest => dest.LoanProgressPercent, opt => opt.Ignore())
            .ForMember(dest => dest.EmiProgressPercent, opt => opt.Ignore())
            .ForMember(
                dest => dest.Reminders,
                opt => opt.MapFrom(src => src.Reminders));

        CreateMap<Loan, LoanListItemResponse>()
            .ForMember(dest => dest.LoanProgressPercent, opt => opt.Ignore());
    }
}
