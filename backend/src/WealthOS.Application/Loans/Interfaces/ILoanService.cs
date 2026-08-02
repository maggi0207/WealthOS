using WealthOS.Application.Common.Models;
using WealthOS.Application.Loans.DTOs.Requests;
using WealthOS.Application.Loans.DTOs.Responses;
using WealthOS.Domain.Loans.Enums;

namespace WealthOS.Application.Loans.Interfaces;

/// <summary>
/// Application service for loan management.
/// </summary>
public interface ILoanService
{
    Task<Result<LoanResponse>> CreateAsync(
        CreateLoanRequest request,
        CancellationToken cancellationToken = default);

    Task<Result<LoanResponse>> UpdateAsync(
        Guid loanId,
        UpdateLoanRequest request,
        CancellationToken cancellationToken = default);

    Task<Result> DeleteAsync(Guid loanId, CancellationToken cancellationToken = default);

    Task<Result<LoanPaymentResponse>> RecordPaymentAsync(
        Guid loanId,
        RecordLoanPaymentRequest request,
        CancellationToken cancellationToken = default);

    Task<Result<LoanResponse>> GetByIdAsync(
        Guid loanId,
        CancellationToken cancellationToken = default);

    Task<Result<LoanListResponse>> GetAllAsync(
        int page,
        int pageSize,
        string? search,
        LoanStatus? status,
        LoanType? type,
        CancellationToken cancellationToken = default);

    Task<Result<LoanSummaryResponse>> GetSummaryAsync(CancellationToken cancellationToken = default);

    Task<Result<UpcomingPaymentsResponse>> GetUpcomingPaymentsAsync(
        int daysAhead = 45,
        int take = 20,
        CancellationToken cancellationToken = default);

    Task<Result<LoanDashboardResponse>> GetDashboardAsync(
        Guid loanId,
        CancellationToken cancellationToken = default);
}
