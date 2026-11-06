using tmb.pedidos.Domain.Entities;
using tmb.pedidos.Domain.Interfaces;
using tmb.pedidos.Domain.Messaging;

namespace tmb.pedidos.API.Services.Orders;

public class CreateOrderService
{
    private readonly IOrderService _orderService;

    public CreateOrderService(
        IOrderService orderService)
    {
        _orderService = orderService;
    }

    public async Task<Order> CreateAsync(
        string customer,
        string product,
        decimal value,
        CancellationToken cancellationToken)
    {
        var order = new Order(
            customer,
            product,
            value);

        var message = new OrderCreatedMessage
        {
            OrderId = order.Id,
            CorrelationId = order.Id,
            EventType = "OrderCreated",
            OccurredAt = DateTime.UtcNow
        };

        await _orderService.InsertWithOutboxAsync(
            order,
            message,
            cancellationToken);

        return order;
    }
}
