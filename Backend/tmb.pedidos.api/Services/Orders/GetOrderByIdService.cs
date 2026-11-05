using tmb.pedidos.Domain.Entities;
using tmb.pedidos.Domain.Interfaces;

namespace tmb.pedidos.API.Services.Orders;

public class GetOrderByIdService
{
    private readonly IOrderService  _orderService;

    public GetOrderByIdService(IOrderService orderService)
    {
        _orderService = orderService;
    }

    public async Task<Order> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _orderService.GetById(id, cancellationToken);
    }
}