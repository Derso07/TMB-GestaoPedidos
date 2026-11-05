using tmb.pedidos.Domain.Entities;
using tmb.pedidos.Domain.Enum;
using tmb.pedidos.Domain.Interfaces;

namespace tmb.pedidos.API.Services.Orders;

public class CreateOrderService
{
    private readonly IOrderService  _orderService;

    public CreateOrderService(IOrderService orderService)
    {
        _orderService = orderService;
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
        return order;
    }
}