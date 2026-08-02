using WealthOS.Application.Common.Abstractions;
using WealthOS.Application.Common.Models;
using WealthOS.Application.Loans.Commands;
using WealthOS.Application.Loans.DTOs.Responses;
using WealthOS.Application.Loans.Interfaces;

namespace WealthOS.Application.Loans.Commands.Handlers;

public sealed class CreateLoanCommandHandler
    : ICommandHandler<CreateLoanCommand, LoanResponse>
{
    private readonly ILoanService _loanService;

    public CreateLoanCommandHandler(ILoanService loanService)
    {
        _loanService = loanService;
    }

    public Task<Result<LoanResponse>> HandleAsync(
        CreateLoanCommand command,
        CancellationToken cancellationToken = default) =>
        _loanService.CreateAsync(command.Request, cancellationToken);
}

public sealed class UpdateLoanCommandHandler
    : ICommandHandler<UpdateLoanCommand, LoanResponse>
{
    private readonly ILoanService _loanService;

    public UpdateLoanCommandHandler(ILoanService loanService)
    {
        _loanService = loanService;
    }

    public Task<Result<LoanResponse>> HandleAsync(
        UpdateLoanCommand command,
        CancellationToken cancellationToken = default) =>
        _loanService.UpdateAsync(command.LoanId, command.Request, cancellationToken);
}

public sealed class DeleteLoanCommandHandler : ICommandHandler<DeleteLoanCommand>
{
    private readonly ILoanService _loanService;

    public DeleteLoanCommandHandler(ILoanService loanService)
    {
        _loanService = loanService;
    }

    public Task<Result> HandleAsync(
        DeleteLoanCommand command,
        CancellationToken cancellationToken = default) =>
        _loanService.DeleteAsync(command.LoanId, cancellationToken);
}

public sealed class RecordPaymentCommandHandler
    : ICommandHandler<RecordPaymentCommand, LoanPaymentResponse>
{
    private readonly ILoanService _loanService;

    public RecordPaymentCommandHandler(ILoanService loanService)
    {
        _loanService = loanService;
    }

    public Task<Result<LoanPaymentResponse>> HandleAsync(
        RecordPaymentCommand command,
        CancellationToken cancellationToken = default) =>
        _loanService.RecordPaymentAsync(command.LoanId, command.Request, cancellationToken);
}
