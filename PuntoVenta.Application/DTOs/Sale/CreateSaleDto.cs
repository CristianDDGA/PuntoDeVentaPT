namespace PuntoVenta.Application.DTOs.Sale;

public class CreateSaleDto
{
    public int                   CustomerId  { get; set; }
    public string                PaymentType { get; set; } = "Efectivo";
    public List<CreateSaleDetailDto> Details { get; set; } = [];
}

public class CreateSaleDetailDto
{
    public int ProductId { get; set; }
    public int Quantity  { get; set; }
}