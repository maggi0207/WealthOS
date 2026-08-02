namespace WealthOS.Application.Common.DTOs;

public sealed class ApiResponse<T>
{
    public bool Success { get; init; }

    public string Message { get; init; } = string.Empty;

    public T? Data { get; init; }

    public IReadOnlyList<ApiErrorDetail> Errors { get; init; } = [];

    public static ApiResponse<T> Ok(T data, string message = "") =>
        new()
        {
            Success = true,
            Message = message,
            Data = data,
            Errors = [],
        };

    public static ApiResponse<T> Fail(string message, IReadOnlyList<ApiErrorDetail>? errors = null) =>
        new()
        {
            Success = false,
            Message = message,
            Data = default,
            Errors = errors ?? [],
        };
}

public sealed class ApiErrorDetail
{
    public string Code { get; init; } = string.Empty;

    public string Message { get; init; } = string.Empty;

    public string? Field { get; init; }
}
