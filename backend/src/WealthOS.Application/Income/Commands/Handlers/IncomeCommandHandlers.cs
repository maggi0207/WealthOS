using WealthOS.Application.Common.Abstractions;
using WealthOS.Application.Common.Models;
using WealthOS.Application.Income.Commands;
using WealthOS.Application.Income.DTOs.Responses;
using WealthOS.Application.Income.Interfaces;

namespace WealthOS.Application.Income.Commands.Handlers;

public sealed class CreateClientCommandHandler : ICommandHandler<CreateClientCommand, ClientResponse>
{
    private readonly IBusinessService _businessService;

    public CreateClientCommandHandler(IBusinessService businessService) => _businessService = businessService;

    public Task<Result<ClientResponse>> HandleAsync(
        CreateClientCommand command,
        CancellationToken cancellationToken = default) =>
        _businessService.CreateClientAsync(command.Request, cancellationToken);
}

public sealed class UpdateClientCommandHandler : ICommandHandler<UpdateClientCommand, ClientResponse>
{
    private readonly IBusinessService _businessService;

    public UpdateClientCommandHandler(IBusinessService businessService) => _businessService = businessService;

    public Task<Result<ClientResponse>> HandleAsync(
        UpdateClientCommand command,
        CancellationToken cancellationToken = default) =>
        _businessService.UpdateClientAsync(command.ClientId, command.Request, cancellationToken);
}

public sealed class DeleteClientCommandHandler : ICommandHandler<DeleteClientCommand>
{
    private readonly IBusinessService _businessService;

    public DeleteClientCommandHandler(IBusinessService businessService) => _businessService = businessService;

    public Task<Result> HandleAsync(
        DeleteClientCommand command,
        CancellationToken cancellationToken = default) =>
        _businessService.DeleteClientAsync(command.ClientId, cancellationToken);
}

public sealed class CreateProjectCommandHandler : ICommandHandler<CreateProjectCommand, ProjectResponse>
{
    private readonly IBusinessService _businessService;

    public CreateProjectCommandHandler(IBusinessService businessService) => _businessService = businessService;

    public Task<Result<ProjectResponse>> HandleAsync(
        CreateProjectCommand command,
        CancellationToken cancellationToken = default) =>
        _businessService.CreateProjectAsync(command.Request, cancellationToken);
}

public sealed class AssignDeveloperCommandHandler : ICommandHandler<AssignDeveloperCommand, ProjectResponse>
{
    private readonly IBusinessService _businessService;

    public AssignDeveloperCommandHandler(IBusinessService businessService) => _businessService = businessService;

    public Task<Result<ProjectResponse>> HandleAsync(
        AssignDeveloperCommand command,
        CancellationToken cancellationToken = default) =>
        _businessService.AssignDeveloperAsync(command.Request, cancellationToken);
}

public sealed class CreateInvoiceCommandHandler : ICommandHandler<CreateInvoiceCommand, InvoiceResponse>
{
    private readonly IInvoiceService _invoiceService;

    public CreateInvoiceCommandHandler(IInvoiceService invoiceService) => _invoiceService = invoiceService;

    public Task<Result<InvoiceResponse>> HandleAsync(
        CreateInvoiceCommand command,
        CancellationToken cancellationToken = default) =>
        _invoiceService.CreateInvoiceAsync(command.Request, cancellationToken);
}

public sealed class RecordInvoicePaymentCommandHandler
    : ICommandHandler<RecordInvoicePaymentCommand, InvoicePaymentResponse>
{
    private readonly IInvoiceService _invoiceService;

    public RecordInvoicePaymentCommandHandler(IInvoiceService invoiceService) => _invoiceService = invoiceService;

    public Task<Result<InvoicePaymentResponse>> HandleAsync(
        RecordInvoicePaymentCommand command,
        CancellationToken cancellationToken = default) =>
        _invoiceService.RecordPaymentAsync(command.Request, cancellationToken);
}

public sealed class CreateExpenseCommandHandler : ICommandHandler<CreateExpenseCommand, ExpenseResponse>
{
    private readonly IBusinessService _businessService;

    public CreateExpenseCommandHandler(IBusinessService businessService) => _businessService = businessService;

    public Task<Result<ExpenseResponse>> HandleAsync(
        CreateExpenseCommand command,
        CancellationToken cancellationToken = default) =>
        _businessService.CreateExpenseAsync(command.Request, cancellationToken);
}

public sealed class RecordSalaryCommandHandler : ICommandHandler<RecordSalaryCommand, SalaryResponse>
{
    private readonly IIncomeService _incomeService;

    public RecordSalaryCommandHandler(IIncomeService incomeService) => _incomeService = incomeService;

    public Task<Result<SalaryResponse>> HandleAsync(
        RecordSalaryCommand command,
        CancellationToken cancellationToken = default) =>
        _incomeService.RecordSalaryAsync(command.Request, cancellationToken);
}

public sealed class CreateDeveloperCommandHandler : ICommandHandler<CreateDeveloperCommand, DeveloperResponse>
{
    private readonly IPayrollService _payrollService;

    public CreateDeveloperCommandHandler(IPayrollService payrollService) => _payrollService = payrollService;

    public Task<Result<DeveloperResponse>> HandleAsync(
        CreateDeveloperCommand command,
        CancellationToken cancellationToken = default) =>
        _payrollService.CreateDeveloperAsync(command.Request, cancellationToken);
}

public sealed class CreatePayrollCommandHandler : ICommandHandler<CreatePayrollCommand, PayrollResponse>
{
    private readonly IPayrollService _payrollService;

    public CreatePayrollCommandHandler(IPayrollService payrollService) => _payrollService = payrollService;

    public Task<Result<PayrollResponse>> HandleAsync(
        CreatePayrollCommand command,
        CancellationToken cancellationToken = default) =>
        _payrollService.CreatePayrollAsync(command.Request, cancellationToken);
}
