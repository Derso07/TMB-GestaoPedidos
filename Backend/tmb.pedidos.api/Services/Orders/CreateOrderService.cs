using tmb.pedidos.Domain.Entities;
using tmb.pedidos.Domain.Enum;
using tmb.pedidos.Domain.Interfaces;
using tmb.pedidos.Domain.Interfaces.MessagingPublish;
using tmb.pedidos.Domain.Messaging;

namespace tmb.pedidos.API.Services.Orders;

public class CreateOrderService
{
    private readonly IOrderService  _orderService;
    private readonly IMessagePublisher _messagePublisher;

    public CreateOrderService(IOrderService orderService, IMessagePublisher messagePublisher)
    {
        _orderService = orderService;
        _messagePublisher = messagePublisher;
    }

    public async Task<Order> CreateAsync(
        string costumer,
        string product,
        decimal value,
        CancellationToken cancellationToken)
    {
        Order order = new Order
        {
            Customer = costumer,
            Product = product,
            Value = value,
            OrderStatus = OrderStatus.pending,
        };

        await _orderService.Insert(order, cancellationToken);

        OrderCreatedMessage message = new OrderCreatedMessage()
        {
            OrderId = order.Id,
            CorrelationId = order.Id,
            EventType = "OrderCreated",
            OccurredAt = DateTime.UtcNow,
        };
        
        await _messagePublisher.PublishOrderCreatedAsync(message, 
            cancellationToken);
        
        return order;
    }
}