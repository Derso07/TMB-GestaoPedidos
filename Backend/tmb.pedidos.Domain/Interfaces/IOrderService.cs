using tmb.pedidos.Domain.Entities;

namespace tmb.pedidos.Domain.Interfaces;

public interface IOrderService
{
    Task Insert(Order order);
    Task<List<Order>> GetAll();
    Task GetById(Guid id);
}