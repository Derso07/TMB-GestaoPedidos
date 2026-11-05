using tmb.pedidos.Domain.Enum;

namespace tmb.pedidos.Domain.Entities;

public class OrderStatusHistory : Entity
{
    public OrderStatusHistory(Guid orderId, OrderStatus? previousStatus, OrderStatus newStatus)
    {
        Id = Guid.NewGuid();
        OrderId = orderId;
        PreviousStatus = previousStatus;
        NewStatus = newStatus;
        ChangeAt = DateTime.UtcNow;
    }

    public Guid OrderId { get; private set; }
    public Order Order { get; private set; }
    public OrderStatus? PreviousStatus { get; private set; }
    public OrderStatus NewStatus { get; private set; }
    public DateTime ChangeAt { get; set; }
}