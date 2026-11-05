using Microsoft.EntityFrameworkCore;
using tmb.pedidos.Domain.Entities;
using tmb.pedidos.Domain.Interfaces;
using tmb.pedidos.Infrastructure.Persistence;

namespace tmb.pedidos.Infrastructure.Repositories;

public class OrderRepository : IOrderService
{
    private readonly PedidosDbContext _pedidosDbContext;

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
        foreach (var entry in _pedidosDbContext.ChangeTracker.Entries())
        {
            Console.WriteLine(
                $"Entity: {entry.Entity.GetType().Name} | State: {entry.State}");
        }
        await _pedidosDbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<Order>> GetAll(CancellationToken cancellationToken)
    {
        return await _pedidosDbContext.Orders
            .OrderByDescending(o => o.CreatOn)
            .ToListAsync(cancellationToken: cancellationToken);
    }

    public async Task<Order?> GetById(Guid id,CancellationToken cancellationToken)
    {
        return await _pedidosDbContext.Orders
            .Include(o => o.StatusHistory)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    }
}