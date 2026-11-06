namespace tmb.pedidos.Domain.Events;

public sealed record OrderCreatedEvent(
    Guid OrderId);