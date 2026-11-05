using tmb.pedidos.Domain.Entities;

namespace tmb.pedidos.Domain.Interfaces;

public interface IOrderService
{
    Task Insert(Order order, CancellationToken cancellationToken);
    Task<List<Order>> GetAll(CancellationToken cancellationToken);
    Task<Order> GetById(Guid id, CancellationToken cancellationToken);
}