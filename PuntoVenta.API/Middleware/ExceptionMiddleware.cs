using System.Net;
using System.Text.Json;
using PuntoVenta.Domain.Exceptions;

namespace PuntoVenta.API.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next   = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (DomainException domainException)
        {
            _logger.LogWarning("Error de dominio: {Message}", domainException.Message);
            await WriteErrorResponseAsync(httpContext, HttpStatusCode.BadRequest, domainException.Message);
        }
        catch (KeyNotFoundException notFoundException)
        {
            _logger.LogWarning("Recurso no encontrado: {Message}", notFoundException.Message);
            await WriteErrorResponseAsync(httpContext, HttpStatusCode.NotFound, notFoundException.Message);
        }
        catch (ArgumentException argumentException)
        {
            _logger.LogWarning("Argumento inválido: {Message}", argumentException.Message);
            await WriteErrorResponseAsync(httpContext, HttpStatusCode.BadRequest, argumentException.Message);
        }
        catch (InvalidOperationException invalidOpException)
        {
            _logger.LogWarning("Operación inválida (Negocio): {Message}", invalidOpException.Message);
            await WriteErrorResponseAsync(httpContext, HttpStatusCode.BadRequest, invalidOpException.Message);
        }
        catch (Exception unexpectedException)
        {
            _logger.LogError(unexpectedException, "Error inesperado.");
            await WriteErrorResponseAsync(httpContext, HttpStatusCode.InternalServerError, "Ocurrió un error interno en el servidor.");
        }
    }

    private static async Task WriteErrorResponseAsync(
        HttpContext httpContext,
        HttpStatusCode statusCode,
        string message)
    {
        httpContext.Response.ContentType = "application/json";
        httpContext.Response.StatusCode  = (int)statusCode;

        var errorResponse = new
        {
            StatusCode = (int)statusCode,
            Message    = message
        };

        await httpContext.Response.WriteAsync(
            JsonSerializer.Serialize(errorResponse));
    }
}