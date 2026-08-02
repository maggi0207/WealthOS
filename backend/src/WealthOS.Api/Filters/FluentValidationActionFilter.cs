using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WealthOS.Application.Common.DTOs;

namespace WealthOS.Api.Filters;

public sealed class FluentValidationActionFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        foreach (var argument in context.ActionArguments.Values)
        {
            if (argument is null)
            {
                continue;
            }

            var validatorType = typeof(IValidator<>).MakeGenericType(argument.GetType());
            if (context.HttpContext.RequestServices.GetService(validatorType) is not IValidator validator)
            {
                continue;
            }

            var validationContext = new ValidationContext<object>(argument);
            var validationResult = await validator.ValidateAsync(validationContext, context.HttpContext.RequestAborted);
            if (validationResult.IsValid)
            {
                continue;
            }

            var errors = validationResult.Errors
                .Select(failure => new ApiErrorDetail
                {
                    Code = "validation_error",
                    Message = failure.ErrorMessage,
                    Field = failure.PropertyName,
                })
                .ToList();

            context.Result = new BadRequestObjectResult(
                ApiResponse<object>.Fail("Validation failed.", errors));
            return;
        }

        await next();
    }
}
