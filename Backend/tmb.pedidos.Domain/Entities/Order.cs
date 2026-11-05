using tmb.pedidos.Domain.Enum;

namespace tmb.pedidos.Domain.Entities;

public class Order : Entity
{
    public string Customer { get; set; }
    public string Product { get; set; }
    public decimal Value { get; set; }
    public DateTime CreatOn { get; set; }
    public OrderStatus OrderStatus { get; set; }
}