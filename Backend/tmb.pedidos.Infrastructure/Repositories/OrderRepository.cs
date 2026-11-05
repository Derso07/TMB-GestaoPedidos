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

    public async Task Insert(Order order)
    {
        await _pedidosDbContext.AddAsync(order);
        await _pedidosDbContext.SaveChangesAsync();
    }

    public async Task<List<Order>> GetAll()
    {
        return await _pedidosDbContext.Orders.ToListAsync();
    }

    public async Task<Order> GetById(Guid id)
    {
        return await _pedidosDbContext.Orders
            .Where(o => o.Id == id)
            .FirstOrDefaultAsync();
    }
}