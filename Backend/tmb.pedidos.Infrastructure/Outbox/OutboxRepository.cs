using Microsoft.EntityFrameworkCore;
using tmb.pedidos.Infrastructure.Persistence;

namespace tmb.pedidos.Infrastructure.Outbox;

public class OutboxRepository : IOutboxRepository
{
    private readonly PedidosDbContext _dbContext;

    public OutboxRepository(
        PedidosDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(
        OutboxMessage message,
        CancellationToken cancellationToken)
    {
        await _dbContext.OutboxMessages.AddAsync(
            message,
            cancellationToken);
    }

    public async Task<IReadOnlyList<OutboxMessage>>
        GetPendingAsync(
            int batchSize,
            CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;

        return await _dbContext.OutboxMessages
            .Where(message =>
                message.ProcessedOnUtc == null &&
                (
                    message.NextAttemptOnUtc == null ||
                    message.NextAttemptOnUtc <= now
                ))
            .OrderBy(message => message.OccurredOnUtc)
            .Take(batchSize)
            .ToListAsync(cancellationToken);
    }

    public Task SaveChangesAsync(
        CancellationToken cancellationToken)
    {
        return _dbContext.SaveChangesAsync(
            cancellationToken);
    }
}