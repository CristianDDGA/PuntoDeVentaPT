namespace PuntoVenta.Blazor.Models;

public class CustomerModel
{
    public int     CustomerId     { get; set; }
    public string  DocumentNumber { get; set; } = string.Empty;
    public string  FirstName      { get; set; } = string.Empty;
    public string  LastName       { get; set; } = string.Empty;
    public string? Phone          { get; set; }
    public string? Address        { get; set; }
    public string? City           { get; set; }
    public string? Email          { get; set; }
    public string  FullName       => $"{FirstName} {LastName}";
}

/// <summary>
/// Payload sent to POST /api/Customers when creating a new customer from the modal form.
/// </summary>
public class CreateCustomerModel
{
    public string  DocumentNumber { get; set; } = string.Empty;
    public string  FirstName      { get; set; } = string.Empty;
    public string  LastName       { get; set; } = string.Empty;
    public string? Phone          { get; set; }
    public string? Address        { get; set; }
    public string? City           { get; set; }
    public string? Email          { get; set; }
}