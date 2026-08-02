using System.Net;
using Microsoft.AspNetCore.Mvc;
using WealthOS.Application.Common.DTOs;
using WealthOS.Application.Common.Models;
using WealthOS.Domain.Common.Exceptions;

namespace WealthOS.Api.Middleware;

public sealed class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(context, exception);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, error, logLevel) = MapException(exception);

        if (logLevel == LogLevel.Error)
        {
            _logger.LogError(exception, "Unhandled exception for {Method} {Path}", context.Request.Method, context.Request.Path);
        }
        else
        {
            _logger.LogWarning(exception, "Handled exception for {Method} {Path}", context.Request.Method, context.Request.Path);
        }

        if (context.Response.HasStarted)
        {
            throw exception;
        }

        context.Response.Clear();
        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/problem+json";

        var problemDetails = new ProblemDetails
        {
            Status = (int)statusCode,
            Title = error.Message,
            Type = $"https://httpstatuses.com/{(int)statusCode}",
            Detail = exception is DomainException ? null : "An unexpected error occurred.",
            Instance = context.Request.Path,
        };

        problemDetails.Extensions["code"] = error.Code;
        problemDetails.Extensions["success"] = false;
        problemDetails.Extensions["errors"] = new[]
        {
            new ApiErrorDetail
            {
                Code = error.Code,
                Message = error.Message,
            },
        };

        if (error.ValidationErrors is not null)
        {
            problemDetails.Extensions["validationErrors"] = error.ValidationErrors;
        }

        await context.Response.WriteAsJsonAsync(problemDetails);
    }

    private static (HttpStatusCode StatusCode, Error Error, LogLevel LogLevel) MapException(Exception exception) =>
        exception switch
        {
            NotFoundException notFound => (HttpStatusCode.NotFound, Error.NotFound(notFound.Resource, notFound.Key), LogLevel.Warning),
            ConflictException conflict => (HttpStatusCode.Conflict, Error.Conflict(conflict.Message), LogLevel.Warning),
            UnauthorizedException unauthorized => (HttpStatusCode.Unauthorized, Error.Unauthorized(unauthorized.Message), LogLevel.Warning),
            ForbiddenException forbidden => (HttpStatusCode.Forbidden, Error.Forbidden(forbidden.Message), LogLevel.Warning),
            DomainException domain => (HttpStatusCode.BadRequest, Error.Failure(domain.Code, domain.Message), LogLevel.Warning),
            _ => (HttpStatusCode.InternalServerError, Error.Failure("internal_error", "An unexpected error occurred."), LogLevel.Error),
        };
}
