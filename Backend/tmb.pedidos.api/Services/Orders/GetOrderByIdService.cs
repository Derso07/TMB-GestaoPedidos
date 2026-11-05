using tmb.pedidos.API.Contracts.Orders;
using tmb.pedidos.Domain.Interfaces;

namespace tmb.pedidos.API.Services.Orders;

public class GetOrderByIdService
{
    private readonly IOrderService  _orderService;

    public GetOrderByIdService(IOrderService orderService)
    {
        _orderService = orderService;
    }

    public async Task<OrderDetailsResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var order = await _orderService.GetById(
            id,
            cancellationToken);

        if (order is null)
        {
            return null;
        }

        return new OrderDetailsResponse
        {
            Id = order.Id,
            Customer = order.Customer,
            Product = order.Product,
            Value = order.Value,
            CreatOn = order.CreatOn,

            // Este campo precisa estar no mapeamento
            OrderStatus = order.OrderStatus.ToString(),

            StatusHistory = order.StatusHistory
                .OrderBy(history => history.ChangeAt)
                .Select(history => new OrderStatusHistoryResponse
                {
                    PreviousStatus =
                        history.PreviousStatus?.ToString(),

                    NewStatus =
                        history.NewStatus.ToString(),

                    ChangedAt =
                        history.ChangeAt
                })
                .ToList()
        };
    }
}