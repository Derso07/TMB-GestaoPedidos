using tmb.pedidos.Domain.Enum;

namespace tmb.pedidos.Domain.Entities;

public class Order : Entity
{
    public Order()
    {}
    
    public Order(string customer, string product, decimal value)
    {
        Customer = customer;
        Product = product;
        Value = value;
        OrderStatus = OrderStatus.Pending;
        CreatOn  = DateTime.UtcNow;
    }
    
    public string Customer { get; set; }
    public string Product { get; set; }
    public decimal Value { get; set; }
    public DateTime CreatOn { get; set; }
    public OrderStatus OrderStatus { get; set; }
}