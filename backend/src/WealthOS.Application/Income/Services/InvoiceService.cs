using AutoMapper;
using WealthOS.Application.Common.Interfaces;
using WealthOS.Application.Common.Models;
using WealthOS.Application.Income.DTOs.Requests;
using WealthOS.Application.Income.DTOs.Responses;
using WealthOS.Application.Income.Interfaces;
using WealthOS.Domain.Common.Abstractions.Repositories;
using WealthOS.Domain.Income.Entities;
using WealthOS.Domain.Income.Enums;
using WealthOS.Domain.Income.Repositories;

namespace WealthOS.Application.Income.Services;

/// <summary>
/// Invoice creation and payment recording.
/// </summary>
public sealed class InvoiceService : IInvoiceService
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IBusinessClientRepository _clientRepository;
    private readonly IBusinessProjectRepository _projectRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;
    private readonly IMapper _mapper;

    public InvoiceService(
        IInvoiceRepository invoiceRepository,
        IBusinessClientRepository clientRepository,
        IBusinessProjectRepository projectRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser,
        IMapper mapper)
    {
        _invoiceRepository = invoiceRepository;
        _clientRepository = clientRepository;
        _projectRepository = projectRepository;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<Result<InvoiceResponse>> CreateInvoiceAsync(
        CreateInvoiceRequest request,
        CancellationToken cancellationToken = default)
    {
        var userResult = RequireUserId();
        if (userResult.IsFailure)
        {
            return Result.Failure<InvoiceResponse>(userResult.Error!);
        }

        var clientExists = await _clientRepository.ExistsForUserAsync(
            request.ClientId,
            userResult.Value,
            cancellationToken);

        if (!clientExists)
        {
            return Result.Failure<InvoiceResponse>(Error.NotFound(nameof(BusinessClient), request.ClientId));
        }

        if (request.ProjectId.HasValue)
        {
            var project = await _projectRepository.GetByIdForUserAsync(
                request.ProjectId.Value,
                userResult.Value,
                cancellationToken);

            if (project is null)
            {
                return Result.Failure<InvoiceResponse>(
                    Error.NotFound(nameof(BusinessProject), request.ProjectId.Value));
            }

            if (project.ClientId != request.ClientId)
            {
                return Result.Failure<InvoiceResponse>(
                    Error.Validation(
                        "Project does not belong to the specified client.",
                        new Dictionary<string, string[]>
                        {
                            ["ProjectId"] = ["Project does not belong to the specified client."],
                        }));
            }
        }

        var items = request.Items.Select(item =>
        {
            var lineTotal = Math.Round(item.Quantity * item.UnitPrice, 2, MidpointRounding.AwayFromZero);
            return new InvoiceItem
            {
                Description = item.Description.Trim(),
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                LineTotal = lineTotal,
            };
        }).ToList();

        var subTotal = items.Sum(item => item.LineTotal);
        var invoice = new Invoice
        {
            UserId = userResult.Value,
            ClientId = request.ClientId,
            ProjectId = request.ProjectId,
            InvoiceNumber = request.InvoiceNumber.Trim(),
            IssueDate = request.IssueDate,
            DueDate = request.DueDate,
            Status = request.Status,
            SubTotal = subTotal,
            AmountPaid = 0m,
            CurrencyCode = NormalizeCurrency(request.CurrencyCode),
            Notes = request.Notes,
            Items = items,
        };

        await _invoiceRepository.AddAsync(invoice, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var created = await _invoiceRepository.GetByIdWithDetailsAsync(
            invoice.Id,
            userResult.Value,
            cancellationToken);

        return Result.Success(_mapper.Map<InvoiceResponse>(created!));
    }

    public async Task<Result<InvoicePaymentResponse>> RecordPaymentAsync(
        RecordInvoicePaymentRequest request,
        CancellationToken cancellationToken = default)
    {
        var userResult = RequireUserId();
        if (userResult.IsFailure)
        {
            return Result.Failure<InvoicePaymentResponse>(userResult.Error!);
        }

        var invoice = await _invoiceRepository.GetByIdWithDetailsAsync(
            request.InvoiceId,
            userResult.Value,
            cancellationToken);

        if (invoice is null)
        {
            return Result.Failure<InvoicePaymentResponse>(
                Error.NotFound(nameof(Invoice), request.InvoiceId));
        }

        if (invoice.Status == InvoiceStatus.Cancelled)
        {
            return Result.Failure<InvoicePaymentResponse>(
                Error.Conflict("Cannot record payment against a cancelled invoice."));
        }

        var outstanding = invoice.OutstandingAmount;
        if (request.Amount > outstanding)
        {
            return Result.Failure<InvoicePaymentResponse>(
                Error.Validation(
                    "Payment exceeds outstanding invoice amount.",
                    new Dictionary<string, string[]>
                    {
                        ["Amount"] = [$"Amount cannot exceed outstanding {outstanding:0.00}."],
                    }));
        }

        var payment = new Payment
        {
            InvoiceId = invoice.Id,
            UserId = userResult.Value,
            Amount = request.Amount,
            PaidOn = request.PaidOn,
            Method = request.Method,
            Reference = request.Reference,
            Notes = request.Notes,
        };

        invoice.Payments.Add(payment);
        invoice.AmountPaid += request.Amount;
        invoice.Status = invoice.AmountPaid >= invoice.SubTotal
            ? InvoiceStatus.Paid
            : InvoiceStatus.PartiallyPaid;

        _invoiceRepository.Update(invoice);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(_mapper.Map<InvoicePaymentResponse>(payment));
    }

    public async Task<Result<InvoiceListResponse>> GetInvoicesAsync(
        int page,
        int pageSize,
        Guid? clientId,
        InvoiceStatus? status,
        string? search,
        CancellationToken cancellationToken = default)
    {
        var userResult = RequireUserId();
        if (userResult.IsFailure)
        {
            return Result.Failure<InvoiceListResponse>(userResult.Error!);
        }

        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var (items, total) = await _invoiceRepository.ListForUserAsync(
            userResult.Value,
            page,
            pageSize,
            clientId,
            status,
            search,
            cancellationToken);

        return Result.Success(new InvoiceListResponse
        {
            Items = _mapper.Map<List<InvoiceResponse>>(items),
            Page = page,
            PageSize = pageSize,
            TotalCount = total,
        });
    }

    public async Task<Result<InvoiceResponse>> GetInvoiceByIdAsync(
        Guid invoiceId,
        CancellationToken cancellationToken = default)
    {
        var userResult = RequireUserId();
        if (userResult.IsFailure)
        {
            return Result.Failure<InvoiceResponse>(userResult.Error!);
        }

        var invoice = await _invoiceRepository.GetByIdWithDetailsAsync(
            invoiceId,
            userResult.Value,
            cancellationToken);

        if (invoice is null)
        {
            return Result.Failure<InvoiceResponse>(Error.NotFound(nameof(Invoice), invoiceId));
        }

        return Result.Success(_mapper.Map<InvoiceResponse>(invoice));
    }

    private Result<Guid> RequireUserId()
    {
        if (!_currentUser.IsAuthenticated || _currentUser.UserId is null)
        {
            return Result.Failure<Guid>(Error.Unauthorized());
        }

        return Result.Success(_currentUser.UserId.Value);
    }

    private static string NormalizeCurrency(string? code) =>
        string.IsNullOrWhiteSpace(code) ? "INR" : code.Trim().ToUpperInvariant();
}
