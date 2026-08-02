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
/// Developer roster and payroll records.
/// </summary>
public sealed class PayrollService : IPayrollService
{
    private readonly IDeveloperRepository _developerRepository;
    private readonly IBusinessClientRepository _clientRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;
    private readonly IMapper _mapper;

    public PayrollService(
        IDeveloperRepository developerRepository,
        IBusinessClientRepository clientRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser,
        IMapper mapper)
    {
        _developerRepository = developerRepository;
        _clientRepository = clientRepository;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<Result<DeveloperResponse>> CreateDeveloperAsync(
        CreateDeveloperRequest request,
        CancellationToken cancellationToken = default)
    {
        var userResult = RequireUserId();
        if (userResult.IsFailure)
        {
            return Result.Failure<DeveloperResponse>(userResult.Error!);
        }

        if (request.PrimaryClientId.HasValue)
        {
            var clientExists = await _clientRepository.ExistsForUserAsync(
                request.PrimaryClientId.Value,
                userResult.Value,
                cancellationToken);

            if (!clientExists)
            {
                return Result.Failure<DeveloperResponse>(
                    Error.NotFound(nameof(BusinessClient), request.PrimaryClientId.Value));
            }
        }

        var developer = _mapper.Map<Developer>(request);
        developer.UserId = userResult.Value;

        await _developerRepository.AddAsync(developer, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var created = await _developerRepository.GetByIdForUserAsync(
            developer.Id,
            userResult.Value,
            cancellationToken);

        return Result.Success(_mapper.Map<DeveloperResponse>(created!));
    }

    public async Task<Result<DeveloperListResponse>> GetDevelopersAsync(
        int page,
        int pageSize,
        string? search,
        bool? isActive,
        CancellationToken cancellationToken = default)
    {
        var userResult = RequireUserId();
        if (userResult.IsFailure)
        {
            return Result.Failure<DeveloperListResponse>(userResult.Error!);
        }

        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var (items, total) = await _developerRepository.ListForUserAsync(
            userResult.Value,
            page,
            pageSize,
            search,
            isActive,
            cancellationToken);

        return Result.Success(new DeveloperListResponse
        {
            Items = _mapper.Map<List<DeveloperResponse>>(items),
            Page = page,
            PageSize = pageSize,
            TotalCount = total,
        });
    }

    public async Task<Result<PayrollResponse>> CreatePayrollAsync(
        CreatePayrollRequest request,
        CancellationToken cancellationToken = default)
    {
        var userResult = RequireUserId();
        if (userResult.IsFailure)
        {
            return Result.Failure<PayrollResponse>(userResult.Error!);
        }

        var developer = await _developerRepository.GetByIdForUserAsync(
            request.DeveloperId,
            userResult.Value,
            cancellationToken);

        if (developer is null)
        {
            return Result.Failure<PayrollResponse>(Error.NotFound(nameof(Developer), request.DeveloperId));
        }

        var payroll = new DeveloperPayroll
        {
            DeveloperId = developer.Id,
            Developer = developer,
            UserId = userResult.Value,
            Amount = request.Amount,
            Period = request.Period.Trim(),
            Status = request.Status,
            PaidOn = request.PaidOn,
            ScheduledOn = request.ScheduledOn,
            Notes = request.Notes,
        };

        developer.PayrollRecords.Add(payroll);
        _developerRepository.Update(developer);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(_mapper.Map<PayrollResponse>(payroll));
    }

    public async Task<Result<PayrollListResponse>> GetPayrollAsync(
        int page,
        int pageSize,
        string? period,
        PayrollStatus? status,
        Guid? developerId,
        CancellationToken cancellationToken = default)
    {
        var userResult = RequireUserId();
        if (userResult.IsFailure)
        {
            return Result.Failure<PayrollListResponse>(userResult.Error!);
        }

        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var (items, total) = await _developerRepository.ListPayrollForUserAsync(
            userResult.Value,
            page,
            pageSize,
            period,
            status,
            developerId,
            cancellationToken);

        return Result.Success(new PayrollListResponse
        {
            Items = _mapper.Map<List<PayrollResponse>>(items),
            Page = page,
            PageSize = pageSize,
            TotalCount = total,
        });
    }

    private Result<Guid> RequireUserId()
    {
        if (!_currentUser.IsAuthenticated || _currentUser.UserId is null)
        {
            return Result.Failure<Guid>(Error.Unauthorized());
        }

        return Result.Success(_currentUser.UserId.Value);
    }
}
