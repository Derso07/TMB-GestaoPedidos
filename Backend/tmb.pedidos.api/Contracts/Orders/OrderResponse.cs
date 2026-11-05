namespace tmb.pedidos.API.Contracts.Orders;

public class OrderResponse
{
    public Guid Id { get; set; }

    public string Customer { get; set; } = string.Empty;

    public string Product { get; set; } = string.Empty;

    public decimal Value { get; set; }

    public DateTime CreatOn { get; set; }

    public string OrderStatus { get; set; } = string.Empty;
}