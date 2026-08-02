using WealthOS.Application.Common.Abstractions;
using WealthOS.Application.Income.DTOs.Requests;

namespace WealthOS.Application.Income.Commands;

public sealed class CreateClientCommand : ICommand
{
    public CreateClientRequest Request { get; init; } = null!;
}

public sealed class UpdateClientCommand : ICommand
{
    public Guid ClientId { get; init; }

    public UpdateClientRequest Request { get; init; } = null!;
}

public sealed class DeleteClientCommand : ICommand
{
    public Guid ClientId { get; init; }
}

public sealed class CreateProjectCommand : ICommand
{
    public CreateProjectRequest Request { get; init; } = null!;
}

public sealed class AssignDeveloperCommand : ICommand
{
    public AssignDeveloperRequest Request { get; init; } = null!;
}

public sealed class CreateInvoiceCommand : ICommand
{
    public CreateInvoiceRequest Request { get; init; } = null!;
}

public sealed class RecordInvoicePaymentCommand : ICommand
{
    public RecordInvoicePaymentRequest Request { get; init; } = null!;
}

public sealed class CreateExpenseCommand : ICommand
{
    public CreateExpenseRequest Request { get; init; } = null!;
}

public sealed class RecordSalaryCommand : ICommand
{
    public RecordSalaryRequest Request { get; init; } = null!;
}

public sealed class CreateDeveloperCommand : ICommand
{
    public CreateDeveloperRequest Request { get; init; } = null!;
}

public sealed class CreatePayrollCommand : ICommand
{
    public CreatePayrollRequest Request { get; init; } = null!;
}
