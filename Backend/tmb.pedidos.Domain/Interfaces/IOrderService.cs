using tmb.pedidos.Domain.Entities;
using tmb.pedidos.Domain.Messaging;

namespace tmb.pedidos.Domain.Interfaces;

public interface IOrderService
{
    Task Insert(Order order, CancellationToken cancellationToken);
    Task Update(Order order, CancellationToken cancellationToken);
    Task<List<Order>> GetAll(CancellationToken cancellationToken);
    Task<Order> GetById(Guid id, CancellationToken cancellationToken);
    Task InsertWithOutboxAsync(Order order, OrderCreatedMessage message, CancellationToken cancellationToken);
}