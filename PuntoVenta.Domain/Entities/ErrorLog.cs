namespace PuntoVenta.Domain.Entities;

public class ErrorLog
{
    public int      ErrorLogId   { get; private set; }
    public string   Message      { get; private set; } = string.Empty;
    public string?  StackTrace   { get; private set; }
    public string?  Path         { get; private set; }
    public string?  HttpMethod   { get; private set; }
    public DateTime OccurredAt   { get; private set; }

    private ErrorLog() { }

    public static ErrorLog Create(string message, string? stackTrace = null, string? path = null, string? httpMethod = null)
    {
        if (string.IsNullOrWhiteSpace(message))
            throw new ArgumentException("El mensaje de error es obligatorio.", nameof(message));

        return new ErrorLog
        {
            Message = TruncateRequired(message.Trim(), 500),
            StackTrace = TruncateOptional(stackTrace?.Trim(), 4000),
            Path = TruncateOptional(path?.Trim(), 250),
            HttpMethod = TruncateOptional(httpMethod?.Trim(), 20),
            OccurredAt = DateTime.UtcNow
        };
    }

    private static string TruncateRequired(string value, int maxLength)
    {
        return value.Length <= maxLength
            ? value
            : value[..maxLength];
    }

    private static string? TruncateOptional(string? value, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        return value.Length <= maxLength
            ? value
            : value[..maxLength];
    }
}