namespace tmb.pedidos.API.Contracts.Orders;

public class OrderStatusHistoryResponse
{
    public string? PreviousStatus { get; set; }

    public string NewStatus { get; set; } = string.Empty;

    public DateTime ChangedAt { get; set; }
}