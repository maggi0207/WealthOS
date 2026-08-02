using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WealthOS.Application.Common.Abstractions;
using WealthOS.Application.Common.DTOs;
using WealthOS.Application.Common.Models;
using WealthOS.Application.Income.Commands;
using WealthOS.Application.Income.DTOs.Requests;
using WealthOS.Application.Income.DTOs.Responses;
using WealthOS.Application.Income.Queries;
using WealthOS.Domain.Income.Enums;

namespace WealthOS.Api.Controllers;

/// <summary>
/// Business client management.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Authorize]
[Route("api/v{version:apiVersion}/clients")]
public sealed class ClientController : ControllerBase
{
    private readonly ICommandHandler<CreateClientCommand, ClientResponse> _createHandler;
    private readonly ICommandHandler<UpdateClientCommand, ClientResponse> _updateHandler;
    private readonly ICommandHandler<DeleteClientCommand> _deleteHandler;
    private readonly IQueryHandler<GetClientsQuery, ClientListResponse> _listHandler;

    /// <summary>
    /// Creates a new <see cref="ClientController"/>.
    /// </summary>
    public ClientController(
        ICommandHandler<CreateClientCommand, ClientResponse> createHandler,
        ICommandHandler<UpdateClientCommand, ClientResponse> updateHandler,
        ICommandHandler<DeleteClientCommand> deleteHandler,
        IQueryHandler<GetClientsQuery, ClientListResponse> listHandler)
    {
        _createHandler = createHandler;
        _updateHandler = updateHandler;
        _deleteHandler = deleteHandler;
        _listHandler = listHandler;
    }

    /// <summary>
    /// Returns a paginated list of business clients.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<ClientListResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] ClientStatus? status = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _listHandler.HandleAsync(
            new GetClientsQuery
            {
                Page = page,
                PageSize = pageSize,
                Search = search,
                Status = status,
            },
            cancellationToken);

        return ToActionResult(result, "Clients retrieved.");
    }

    /// <summary>
    /// Creates a business client.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<ClientResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Create(
        [FromBody] CreateClientRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _createHandler.HandleAsync(
            new CreateClientCommand { Request = request },
            cancellationToken);

        if (result.IsSuccess)
        {
            return StatusCode(
                StatusCodes.Status201Created,
                ApiResponse<ClientResponse>.Ok(result.Value, "Client created."));
        }

        return IncomeControllerHelpers.ToFailureResult(this, result.Error!);
    }

    /// <summary>
    /// Updates a business client.
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<ClientResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateClientRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _updateHandler.HandleAsync(
            new UpdateClientCommand { ClientId = id, Request = request },
            cancellationToken);

        return ToActionResult(result, "Client updated.");
    }

    /// <summary>
    /// Soft-deletes a business client.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _deleteHandler.HandleAsync(
            new DeleteClientCommand { ClientId = id },
            cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(ApiResponse<object?>.Ok(null, "Client deleted."));
        }

        return IncomeControllerHelpers.ToFailureResult(this, result.Error!);
    }

    private IActionResult ToActionResult<T>(Result<T> result, string successMessage)
    {
        if (result.IsSuccess)
        {
            return Ok(ApiResponse<T>.Ok(result.Value, successMessage));
        }

        return IncomeControllerHelpers.ToFailureResult(this, result.Error!);
    }
}
