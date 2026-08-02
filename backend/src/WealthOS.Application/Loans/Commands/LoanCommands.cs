using WealthOS.Application.Common.Abstractions;
using WealthOS.Application.Loans.DTOs.Requests;

namespace WealthOS.Application.Loans.Commands;

/// <summary>
/// Creates a new loan for the authenticated user.
/// </summary>
public sealed class CreateLoanCommand : ICommand
{
    public CreateLoanRequest Request { get; init; } = null!;
}

/// <summary>
/// Updates an existing loan owned by the authenticated user.
/// </summary>
public sealed class UpdateLoanCommand : ICommand
{
    public Guid LoanId { get; init; }

    public UpdateLoanRequest Request { get; init; } = null!;
}

/// <summary>
/// Soft-deletes a loan owned by the authenticated user.
/// </summary>
public sealed class DeleteLoanCommand : ICommand
{
    public Guid LoanId { get; init; }
}

/// <summary>
/// Records a payment against a loan.
/// </summary>
public sealed class RecordPaymentCommand : ICommand
{
    public Guid LoanId { get; init; }

    public RecordLoanPaymentRequest Request { get; init; } = null!;
}
