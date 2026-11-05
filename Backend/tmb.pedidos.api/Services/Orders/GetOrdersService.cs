using tmb.pedidos.Domain.Entities;
using tmb.pedidos.Domain.Interfaces;

namespace tmb.pedidos.API.Services.Orders;

public class GetOrdersService
{
    private readonly IOrderService  _orderService;

    public GetOrdersService(IOrderService orderService)
    {
        _orderService = orderService;
    }

    public async Task<List<Order>> GetOrdersAsync(CancellationToken cancellationToken)
    {
        return await _orderService.GetAll(cancellationToken);
    }
}