using Microsoft.EntityFrameworkCore;
using tmb.pedidos.Domain.Entities;
using tmb.pedidos.Domain.Interfaces;
using tmb.pedidos.Infrastructure.Persistence;

namespace tmb.pedidos.Infrastructure.Repositories;

public class OrderRepository : IOrderService
{
    private readonly PedidosDbContext  _pedidosDbContext;

    public OrderRepository(PedidosDbContext pedidosDbContext)
    {
        _pedidosDbContext = pedidosDbContext;
    }

    public async Task Insert(Order order, CancellationToken  cancellationToken)
    {
        await _pedidosDbContext.AddAsync(order,cancellationToken);
        await _pedidosDbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task Update(Order order, CancellationToken cancellationToken)
    {
        var _order = await GetById(order.Id, cancellationToken);
        
        _pedidosDbContext.Update(_order);
        await _pedidosDbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<Order>> GetAll(CancellationToken cancellationToken)
    {
        return await _pedidosDbContext.Orders.ToListAsync(cancellationToken: cancellationToken);
    }

    public async Task<Order?> GetById(Guid id,CancellationToken cancellationToken)
    {
        return await _pedidosDbContext.Orders
            .Where(o => o.Id == id)
            .FirstOrDefaultAsync();
    }
}