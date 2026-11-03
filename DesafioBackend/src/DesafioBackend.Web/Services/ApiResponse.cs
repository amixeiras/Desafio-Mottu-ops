using System.Net;

namespace DesafioBackend.Web.Services;

public class ApiResponse
{
    public bool IsSuccess { get; init; }
    public HttpStatusCode StatusCode { get; init; }
    public string? ErrorMessage { get; init; }

    public static ApiResponse Success(HttpStatusCode statusCode) => new() { IsSuccess = true, StatusCode = statusCode };
    public static ApiResponse Failure(HttpStatusCode statusCode, string errorMessage) => new() { IsSuccess = false, StatusCode = statusCode, ErrorMessage = errorMessage };
}

public class ApiResponse<T> : ApiResponse
{
    public T? Data { get; init; }

    public static ApiResponse<T> Success(T data, HttpStatusCode statusCode) => new() { IsSuccess = true, StatusCode = statusCode, Data = data };
    public new static ApiResponse<T> Failure(HttpStatusCode statusCode, string errorMessage) => new() { IsSuccess = false, StatusCode = statusCode, ErrorMessage = errorMessage };
}

