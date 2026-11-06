using tmb.pedidos.Infrastructure.Outbox;

public interface IOutboxRepository
{
    Task AddAsync(
        OutboxMessage message,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<OutboxMessage>> GetPendingAsync(
        int batchSize,
        CancellationToken cancellationToken);

    Task SaveChangesAsync(
        CancellationToken cancellationToken);
}