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
/// Clients, projects, developer assignment, and business expenses.
/// </summary>
public sealed class BusinessService : IBusinessService
{
    private readonly IBusinessClientRepository _clientRepository;
    private readonly IBusinessProjectRepository _projectRepository;
    private readonly IDeveloperRepository _developerRepository;
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IBusinessExpenseRepository _expenseRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;
    private readonly IMapper _mapper;

    public BusinessService(
        IBusinessClientRepository clientRepository,
        IBusinessProjectRepository projectRepository,
        IDeveloperRepository developerRepository,
        IInvoiceRepository invoiceRepository,
        IBusinessExpenseRepository expenseRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser,
        IMapper mapper)
    {
        _clientRepository = clientRepository;
        _projectRepository = projectRepository;
        _developerRepository = developerRepository;
        _invoiceRepository = invoiceRepository;
        _expenseRepository = expenseRepository;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<Result<ClientResponse>> CreateClientAsync(
        CreateClientRequest request,
        CancellationToken cancellationToken = default)
    {
        var userResult = RequireUserId();
        if (userResult.IsFailure)
        {
            return Result.Failure<ClientResponse>(userResult.Error!);
        }

        var client = _mapper.Map<BusinessClient>(request);
        client.UserId = userResult.Value;

        await _clientRepository.AddAsync(client, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(MapClient(client, 0m, 0m, null));
    }

    public async Task<Result<ClientResponse>> UpdateClientAsync(
        Guid clientId,
        UpdateClientRequest request,
        CancellationToken cancellationToken = default)
    {
        var userResult = RequireUserId();
        if (userResult.IsFailure)
        {
            return Result.Failure<ClientResponse>(userResult.Error!);
        }

        var client = await _clientRepository.GetByIdForUserAsync(clientId, userResult.Value, cancellationToken);
        if (client is null)
        {
            return Result.Failure<ClientResponse>(Error.NotFound(nameof(BusinessClient), clientId));
        }

        client.Name = request.Name.Trim();
        client.Engagement = request.Engagement.Trim();
        client.Status = request.Status;
        client.MonthlyRevenue = request.MonthlyRevenue;
        client.CurrencyCode = NormalizeCurrency(request.CurrencyCode);
        client.ContactEmail = request.ContactEmail;
        client.ContactPhone = request.ContactPhone;
        client.Notes = request.Notes;

        _clientRepository.Update(client);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var stats = await _invoiceRepository.GetClientPaymentStatsAsync(userResult.Value, cancellationToken);
        stats.TryGetValue(client.Id, out var clientStats);

        return Result.Success(MapClient(
            client,
            clientStats.Outstanding,
            clientStats.LastPaymentAmount,
            clientStats.LastPaymentOn));
    }

    public async Task<Result> DeleteClientAsync(Guid clientId, CancellationToken cancellationToken = default)
    {
        var userResult = RequireUserId();
        if (userResult.IsFailure)
        {
            return Result.Failure(userResult.Error!);
        }

        var client = await _clientRepository.GetByIdForUserAsync(clientId, userResult.Value, cancellationToken);
        if (client is null)
        {
            return Result.Failure(Error.NotFound(nameof(BusinessClient), clientId));
        }

        _clientRepository.Remove(client);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result<ClientListResponse>> GetClientsAsync(
        int page,
        int pageSize,
        string? search,
        ClientStatus? status,
        CancellationToken cancellationToken = default)
    {
        var userResult = RequireUserId();
        if (userResult.IsFailure)
        {
            return Result.Failure<ClientListResponse>(userResult.Error!);
        }

        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var (items, total) = await _clientRepository.ListForUserAsync(
            userResult.Value,
            page,
            pageSize,
            search,
            status,
            cancellationToken);

        var stats = await _invoiceRepository.GetClientPaymentStatsAsync(userResult.Value, cancellationToken);

        var responses = items.Select(client =>
        {
            stats.TryGetValue(client.Id, out var clientStats);
            return MapClient(
                client,
                clientStats.Outstanding,
                clientStats.LastPaymentAmount,
                clientStats.LastPaymentOn);
        }).ToList();

        return Result.Success(new ClientListResponse
        {
            Items = responses,
            Page = page,
            PageSize = pageSize,
            TotalCount = total,
        });
    }

    public async Task<Result<ProjectResponse>> CreateProjectAsync(
        CreateProjectRequest request,
        CancellationToken cancellationToken = default)
    {
        var userResult = RequireUserId();
        if (userResult.IsFailure)
        {
            return Result.Failure<ProjectResponse>(userResult.Error!);
        }

        var clientExists = await _clientRepository.ExistsForUserAsync(
            request.ClientId,
            userResult.Value,
            cancellationToken);

        if (!clientExists)
        {
            return Result.Failure<ProjectResponse>(Error.NotFound(nameof(BusinessClient), request.ClientId));
        }

        var project = _mapper.Map<BusinessProject>(request);
        project.UserId = userResult.Value;

        await _projectRepository.AddAsync(project, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var created = await _projectRepository.GetByIdWithDevelopersAsync(
            project.Id,
            userResult.Value,
            cancellationToken);

        return Result.Success(MapProject(created!));
    }

    public async Task<Result<ProjectResponse>> AssignDeveloperAsync(
        AssignDeveloperRequest request,
        CancellationToken cancellationToken = default)
    {
        var userResult = RequireUserId();
        if (userResult.IsFailure)
        {
            return Result.Failure<ProjectResponse>(userResult.Error!);
        }

        var project = await _projectRepository.GetByIdWithDevelopersAsync(
            request.ProjectId,
            userResult.Value,
            cancellationToken);

        if (project is null)
        {
            return Result.Failure<ProjectResponse>(Error.NotFound(nameof(BusinessProject), request.ProjectId));
        }

        var developerExists = await _developerRepository.ExistsForUserAsync(
            request.DeveloperId,
            userResult.Value,
            cancellationToken);

        if (!developerExists)
        {
            return Result.Failure<ProjectResponse>(Error.NotFound(nameof(Developer), request.DeveloperId));
        }

        var existing = project.Developers.FirstOrDefault(link =>
            link.DeveloperId == request.DeveloperId && !link.IsDeleted);

        if (existing is not null)
        {
            existing.IsActive = true;
            existing.AssignedOn = request.AssignedOn;
            existing.RoleOnProject = request.RoleOnProject;
        }
        else
        {
            project.Developers.Add(new ProjectDeveloper
            {
                ProjectId = project.Id,
                DeveloperId = request.DeveloperId,
                AssignedOn = request.AssignedOn,
                RoleOnProject = request.RoleOnProject,
                IsActive = true,
            });
        }

        _projectRepository.Update(project);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var refreshed = await _projectRepository.GetByIdWithDevelopersAsync(
            project.Id,
            userResult.Value,
            cancellationToken);

        return Result.Success(MapProject(refreshed!));
    }

    public async Task<Result<ProjectListResponse>> GetProjectsAsync(
        int page,
        int pageSize,
        Guid? clientId,
        ProjectStatus? status,
        string? search,
        CancellationToken cancellationToken = default)
    {
        var userResult = RequireUserId();
        if (userResult.IsFailure)
        {
            return Result.Failure<ProjectListResponse>(userResult.Error!);
        }

        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var (items, total) = await _projectRepository.ListForUserAsync(
            userResult.Value,
            page,
            pageSize,
            clientId,
            status,
            search,
            cancellationToken);

        return Result.Success(new ProjectListResponse
        {
            Items = items.Select(MapProject).ToList(),
            Page = page,
            PageSize = pageSize,
            TotalCount = total,
        });
    }

    public async Task<Result<ExpenseResponse>> CreateExpenseAsync(
        CreateExpenseRequest request,
        CancellationToken cancellationToken = default)
    {
        var userResult = RequireUserId();
        if (userResult.IsFailure)
        {
            return Result.Failure<ExpenseResponse>(userResult.Error!);
        }

        ExpenseCategory? category = null;
        if (request.CategoryId.HasValue)
        {
            category = await _expenseRepository.GetCategoryByIdForUserAsync(
                request.CategoryId.Value,
                userResult.Value,
                cancellationToken);

            if (category is null)
            {
                return Result.Failure<ExpenseResponse>(
                    Error.NotFound(nameof(ExpenseCategory), request.CategoryId.Value));
            }
        }
        else
        {
            var name = request.CategoryName.Trim();
            category = await _expenseRepository.FindCategoryByNameAsync(
                userResult.Value,
                name,
                cancellationToken);

            if (category is null)
            {
                category = new ExpenseCategory
                {
                    UserId = userResult.Value,
                    Name = name,
                    IsSystem = false,
                };
                await _expenseRepository.AddCategoryAsync(category, cancellationToken);
            }
        }

        var period = $"{request.PaidOn.Year:D4}-{request.PaidOn.Month:D2}";
        var expense = new BusinessExpense
        {
            UserId = userResult.Value,
            CategoryId = category.Id,
            Category = category,
            Vendor = request.Vendor.Trim(),
            Amount = request.Amount,
            CurrencyCode = NormalizeCurrency(request.CurrencyCode),
            PaidOn = request.PaidOn,
            IsRecurring = request.IsRecurring,
            Period = period,
            Notes = request.Notes,
        };

        await _expenseRepository.AddAsync(expense, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(_mapper.Map<ExpenseResponse>(expense));
    }

    public async Task<Result<ExpenseListResponse>> GetExpensesAsync(
        int page,
        int pageSize,
        Guid? categoryId,
        string? period,
        CancellationToken cancellationToken = default)
    {
        var userResult = RequireUserId();
        if (userResult.IsFailure)
        {
            return Result.Failure<ExpenseListResponse>(userResult.Error!);
        }

        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var (items, total) = await _expenseRepository.ListForUserAsync(
            userResult.Value,
            page,
            pageSize,
            categoryId,
            period,
            cancellationToken);

        return Result.Success(new ExpenseListResponse
        {
            Items = _mapper.Map<List<ExpenseResponse>>(items),
            Page = page,
            PageSize = pageSize,
            TotalCount = total,
        });
    }

    private static ClientResponse MapClient(
        BusinessClient client,
        decimal outstanding,
        decimal lastPaymentAmount,
        DateOnly? lastPaymentOn) =>
        new()
        {
            Id = client.Id,
            Name = client.Name,
            Engagement = client.Engagement,
            Status = client.Status,
            MonthlyRevenue = client.MonthlyRevenue,
            OutstandingInvoice = outstanding,
            LastPaymentAmount = lastPaymentAmount,
            LastPaymentOn = lastPaymentOn,
            CurrencyCode = client.CurrencyCode,
            ContactEmail = client.ContactEmail,
            ContactPhone = client.ContactPhone,
            Notes = client.Notes,
        };

    private static ProjectResponse MapProject(BusinessProject project) =>
        new()
        {
            Id = project.Id,
            ClientId = project.ClientId,
            ClientName = project.Client?.Name ?? string.Empty,
            Name = project.Name,
            Description = project.Description,
            Status = project.Status,
            StartDate = project.StartDate,
            EndDate = project.EndDate,
            MonthlyRevenue = project.MonthlyRevenue,
            CurrencyCode = project.CurrencyCode,
            Developers = project.Developers
                .Where(link => link.IsActive && !link.IsDeleted)
                .Select(link => new ProjectDeveloperResponse
                {
                    DeveloperId = link.DeveloperId,
                    DeveloperName = link.Developer?.Name ?? string.Empty,
                    RoleOnProject = link.RoleOnProject,
                    AssignedOn = link.AssignedOn,
                    IsActive = link.IsActive,
                })
                .ToList(),
        };

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
