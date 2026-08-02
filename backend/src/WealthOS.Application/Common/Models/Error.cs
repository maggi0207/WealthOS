namespace WealthOS.Application.Common.Models;

public sealed class Error
{
    public string Code { get; }

    public string Message { get; }

    public IReadOnlyDictionary<string, string[]>? ValidationErrors { get; }

    private Error(string code, string message, IReadOnlyDictionary<string, string[]>? validationErrors = null)
    {
        Code = code;
        Message = message;
        ValidationErrors = validationErrors;
    }

    public static Error Failure(string code, string message) => new(code, message);

    public static Error Validation(string message, IReadOnlyDictionary<string, string[]> validationErrors) =>
        new("validation_error", message, validationErrors);

    public static Error NotFound(string resource, object key) =>
        Failure("not_found", $"{resource} with identifier '{key}' was not found.");

    public static Error Unauthorized(string message = "Authentication is required.") =>
        Failure("unauthorized", message);

    public static Error Forbidden(string message = "You do not have permission to perform this action.") =>
        Failure("forbidden", message);

    public static Error Conflict(string message) => Failure("conflict", message);
}
