namespace tmb.pedidos.Domain.Messaging;

public class OrderCreatedMessage
{
    public Guid OrderId { get; set; }

    public Guid CorrelationId { get; set; }

    public string EventType { get; set; } = "OrderCreated";

    public DateTime OccurredAt { get; set; }
}