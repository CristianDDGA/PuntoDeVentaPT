namespace PuntoVenta.Application.DTOs.ErrorLog;

public class ErrorLogDto
{
    public int      ErrorLogId   { get; set; }
    public string   Message      { get; set; } = string.Empty;
    public string?  StackTrace   { get; set; }
    public string?  Path         { get; set; }
    public string?  HttpMethod   { get; set; }
    public DateTime OccurredAt   { get; set; }
}
