using System.Net;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using PuntoVenta.Domain.Entities;
using PuntoVenta.Domain.Exceptions;
using PuntoVenta.Infrastructure.Persistence;

namespace PuntoVenta.API.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    public ExceptionMiddleware(
        RequestDelegate next,
        ILogger<ExceptionMiddleware> logger,
        IHostEnvironment environment)
    {
        _next        = next;
        _logger      = logger;
        _environment = environment;
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
            await PersistErrorLogAsync(httpContext, domainException);
            if (_environment.IsDevelopment())
                await WriteDetailedErrorResponseAsync(httpContext, HttpStatusCode.BadRequest, domainException);
            else
                await WriteErrorResponseAsync(httpContext, HttpStatusCode.BadRequest, domainException.Message);
        }
        catch (KeyNotFoundException notFoundException)
        {
            _logger.LogWarning("Recurso no encontrado: {Message}", notFoundException.Message);
            await PersistErrorLogAsync(httpContext, notFoundException);
            if (_environment.IsDevelopment())
                await WriteDetailedErrorResponseAsync(httpContext, HttpStatusCode.NotFound, notFoundException);
            else
                await WriteErrorResponseAsync(httpContext, HttpStatusCode.NotFound, notFoundException.Message);
        }
        catch (ArgumentException argumentException)
        {
            _logger.LogWarning("Argumento inválido: {Message}", argumentException.Message);
            await PersistErrorLogAsync(httpContext, argumentException);
            if (_environment.IsDevelopment())
                await WriteDetailedErrorResponseAsync(httpContext, HttpStatusCode.BadRequest, argumentException);
            else
                await WriteErrorResponseAsync(httpContext, HttpStatusCode.BadRequest, argumentException.Message);
        }
        catch (InvalidOperationException invalidOpException)
        {
            _logger.LogWarning("Operación inválida (Negocio): {Message}", invalidOpException.Message);
            await PersistErrorLogAsync(httpContext, invalidOpException);
            if (_environment.IsDevelopment())
                await WriteDetailedErrorResponseAsync(httpContext, HttpStatusCode.BadRequest, invalidOpException);
            else
                await WriteErrorResponseAsync(httpContext, HttpStatusCode.BadRequest, invalidOpException.Message);
        }
        catch (Exception unexpectedException)
        {
            _logger.LogError(unexpectedException, "Error inesperado.");
            await PersistErrorLogAsync(httpContext, unexpectedException);
            if (_environment.IsDevelopment())
                await WriteDetailedErrorResponseAsync(httpContext, HttpStatusCode.InternalServerError, unexpectedException);
            else
                await WriteErrorResponseAsync(httpContext, HttpStatusCode.InternalServerError, "Ocurrió un error interno en el servidor.");
        }
    }

    private async Task PersistErrorLogAsync(HttpContext httpContext, Exception exception)
    {
        try
        {
            var appDbContext = httpContext.RequestServices.GetRequiredService<AppDbContext>();

            var errorLog = ErrorLog.Create(
                exception.Message,
                exception.ToString(),
                httpContext.Request.Path.Value,
                httpContext.Request.Method);

            await appDbContext.ErrorLogs.AddAsync(errorLog);
            await appDbContext.SaveChangesAsync();
        }
        catch (Exception loggingException)
        {
            _logger.LogError(loggingException, "No se pudo guardar el ErrorLog.");
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

    private async Task WriteDetailedErrorResponseAsync(
    HttpContext httpContext,
    HttpStatusCode statusCode,
    Exception exception)
    {
        httpContext.Response.ContentType = "application/json";
        httpContext.Response.StatusCode = (int)statusCode;

        var errorResponse = new
        {
            StatusCode = (int)statusCode,
            Message = exception.Message,
            // 👇 ESTA LÍNEA TE REVELARÁ EL ERROR REAL DE ORACLE (ORA-XXXXX)
            InnerError = exception.InnerException?.Message,
            Exception = exception.GetType().Name,
            StackTrace = exception.StackTrace
        };

        await httpContext.Response.WriteAsync(
            JsonSerializer.Serialize(errorResponse));
    }
}