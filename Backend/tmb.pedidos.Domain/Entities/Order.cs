using tmb.pedidos.Domain.Enum;

namespace tmb.pedidos.Domain.Entities;

public class Order : Entity
{
    private Order()
    {}
    
    public Order(string customer, string product, decimal value)
    {
        Customer = customer;
        Product = product;
        Value = value;
        OrderStatus = OrderStatus.Pending;
        CreatOn  = DateTime.UtcNow;
        
        StatusHistory.Add(
            new OrderStatusHistory(
                Id,
                previousStatus: null,
                newStatus: OrderStatus.Pending));
    }
    
    public string Customer { get; private set; }
    public string Product { get; private set; }
    public decimal Value { get; private set; }
    public DateTime CreatOn { get; private set; }
    public OrderStatus OrderStatus { get; private set; }
    
    public ICollection<OrderStatusHistory> StatusHistory { get; private set; } =  new List<OrderStatusHistory>();

    public void ChangeStatus(OrderStatus newStatus)
    {
        if (OrderStatus == newStatus)
        {
            return;
        }

        ValidateTransition(OrderStatus, newStatus);

        var previousStatus = OrderStatus;

        OrderStatus = newStatus;

        StatusHistory.Add(
            new OrderStatusHistory(
                Id,
                previousStatus,
                newStatus));
    }

    private static void ValidateTransition(
            OrderStatus currentStatus,
            OrderStatus newStatus)
        {
            var isValid =
                currentStatus == OrderStatus.Pending &&
                newStatus == OrderStatus.Processing
                ||
                currentStatus == OrderStatus.Processing &&
                newStatus == OrderStatus.Completed;

            if (!isValid)
            {
                throw new InvalidOperationException(
                    $"Transição inválida: {currentStatus} -> {newStatus}.");
            }
        }
    }
