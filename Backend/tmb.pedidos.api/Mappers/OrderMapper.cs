using tmb.pedidos.API.Contracts.Orders;
using tmb.pedidos.Domain.Entities;

namespace tmb.pedidos.API.Mappers;

public static class OrderMapper
{
    public static Order ToEntity(OrderRequest request)
    {
        return new Order(
            request.Customer,
            request.Product,
            request.Value);
    }

    public static OrderResponse ToResponse(Order order)
    {
        return new OrderResponse
        {
            Id = order.Id,
            Customer = order.Customer,
            Product = order.Product,
            Value = order.Value,
            CreatOn = order.CreatOn,
            OrderStatus = order.OrderStatus.ToString()
        };
    }

    public static IReadOnlyList<OrderResponse> ToResponse(
        IEnumerable<Order> orders)
    {
        return orders
            .Select(ToResponse)
            .ToList();
    }

    public static OrderDetailsResponse ToDetailsResponse(Order order)
    {
        return new OrderDetailsResponse
        {
            Id = order.Id,
            Customer = order.Customer,
            Product = order.Product,
            Value = order.Value,
            CreatOn = order.CreatOn,
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