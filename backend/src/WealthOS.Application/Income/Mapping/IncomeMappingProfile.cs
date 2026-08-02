using AutoMapper;
using WealthOS.Application.Income.DTOs.Requests;
using WealthOS.Application.Income.DTOs.Responses;
using WealthOS.Domain.Income.Entities;

namespace WealthOS.Application.Income.Mapping;

/// <summary>
/// AutoMapper profile for Income &amp; Business entities and DTOs.
/// </summary>
public sealed class IncomeMappingProfile : Profile
{
    public IncomeMappingProfile()
    {
        CreateMap<CreateClientRequest, BusinessClient>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.UserId, o => o.Ignore())
            .ForMember(d => d.Projects, o => o.Ignore())
            .ForMember(d => d.Invoices, o => o.Ignore())
            .ForMember(d => d.CreatedAt, o => o.Ignore())
            .ForMember(d => d.CreatedBy, o => o.Ignore())
            .ForMember(d => d.UpdatedAt, o => o.Ignore())
            .ForMember(d => d.UpdatedBy, o => o.Ignore())
            .ForMember(d => d.IsDeleted, o => o.Ignore())
            .ForMember(d => d.DeletedAt, o => o.Ignore())
            .ForMember(d => d.Name, o => o.MapFrom(s => s.Name.Trim()))
            .ForMember(d => d.Engagement, o => o.MapFrom(s => s.Engagement.Trim()))
            .ForMember(d => d.CurrencyCode, o => o.MapFrom(s => NormalizeCurrency(s.CurrencyCode)));

        CreateMap<CreateProjectRequest, BusinessProject>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.UserId, o => o.Ignore())
            .ForMember(d => d.Client, o => o.Ignore())
            .ForMember(d => d.Developers, o => o.Ignore())
            .ForMember(d => d.Invoices, o => o.Ignore())
            .ForMember(d => d.CreatedAt, o => o.Ignore())
            .ForMember(d => d.CreatedBy, o => o.Ignore())
            .ForMember(d => d.UpdatedAt, o => o.Ignore())
            .ForMember(d => d.UpdatedBy, o => o.Ignore())
            .ForMember(d => d.IsDeleted, o => o.Ignore())
            .ForMember(d => d.DeletedAt, o => o.Ignore())
            .ForMember(d => d.Name, o => o.MapFrom(s => s.Name.Trim()))
            .ForMember(d => d.CurrencyCode, o => o.MapFrom(s => NormalizeCurrency(s.CurrencyCode)));

        CreateMap<CreateDeveloperRequest, Developer>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.UserId, o => o.Ignore())
            .ForMember(d => d.PrimaryClient, o => o.Ignore())
            .ForMember(d => d.ProjectAssignments, o => o.Ignore())
            .ForMember(d => d.PayrollRecords, o => o.Ignore())
            .ForMember(d => d.IsActive, o => o.MapFrom(_ => true))
            .ForMember(d => d.CreatedAt, o => o.Ignore())
            .ForMember(d => d.CreatedBy, o => o.Ignore())
            .ForMember(d => d.UpdatedAt, o => o.Ignore())
            .ForMember(d => d.UpdatedBy, o => o.Ignore())
            .ForMember(d => d.IsDeleted, o => o.Ignore())
            .ForMember(d => d.DeletedAt, o => o.Ignore())
            .ForMember(d => d.Name, o => o.MapFrom(s => s.Name.Trim()))
            .ForMember(d => d.Role, o => o.MapFrom(s => s.Role.Trim()))
            .ForMember(d => d.CurrencyCode, o => o.MapFrom(s => NormalizeCurrency(s.CurrencyCode)));

        CreateMap<InvoiceItem, InvoiceItemResponse>();
        CreateMap<Payment, InvoicePaymentResponse>();

        CreateMap<Invoice, InvoiceResponse>()
            .ForMember(d => d.ClientName, o => o.MapFrom(s => s.Client != null ? s.Client.Name : string.Empty))
            .ForMember(d => d.OutstandingAmount, o => o.MapFrom(s => s.OutstandingAmount));

        CreateMap<BusinessExpense, ExpenseResponse>()
            .ForMember(d => d.CategoryName, o => o.MapFrom(s => s.Category != null ? s.Category.Name : string.Empty));

        CreateMap<Developer, DeveloperResponse>()
            .ForMember(d => d.PrimaryClientName, o => o.MapFrom(s => s.PrimaryClient != null ? s.PrimaryClient.Name : null));

        CreateMap<DeveloperPayroll, PayrollResponse>()
            .ForMember(d => d.DeveloperName, o => o.MapFrom(s => s.Developer != null ? s.Developer.Name : string.Empty));

        CreateMap<Salary, SalaryResponse>()
            .ForMember(d => d.PaymentId, o => o.Ignore());
    }

    private static string NormalizeCurrency(string? code) =>
        string.IsNullOrWhiteSpace(code) ? "INR" : code.Trim().ToUpperInvariant();
}
