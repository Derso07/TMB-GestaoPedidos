namespace tmb.pedidos.API.Contracts.Orders;

public class OrderDetailsResponse : OrderResponse
{
    public List<OrderStatusHistoryResponse> StatusHistory { get; set; } =
        new();
}