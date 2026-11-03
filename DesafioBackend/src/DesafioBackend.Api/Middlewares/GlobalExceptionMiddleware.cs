using System.Net;
using System.Text.Json;
using DesafioBackend.Application.Common.Exceptions;
using DesafioBackend.Domain.Exceptions;

namespace DesafioBackend.Api.Middlewares;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate _next, ILogger<GlobalExceptionMiddleware> logger)
    {
        this._next = _next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ocorreu um erro não tratado: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var (statusCode, message) = exception switch
        {
            DomainException de => (HttpStatusCode.BadRequest, de.Message),
            ValidationException ve => (HttpStatusCode.BadRequest, ve.Message),
            NotFoundException nfe => (HttpStatusCode.NotFound, nfe.Message),
            ConflictException ce => (HttpStatusCode.Conflict, ce.Message),
            _ => (HttpStatusCode.InternalServerError, "Ocorreu um erro interno no servidor.")
        };

        context.Response.StatusCode = (int)statusCode;

        var response = new
        {
            mensagem = message
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}

