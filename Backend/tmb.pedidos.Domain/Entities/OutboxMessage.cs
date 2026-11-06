namespace tmb.pedidos.Infrastructure.Outbox;

public class OutboxMessage
{
    private OutboxMessage()
    {
    }

    public OutboxMessage(
        string type,
        string payload)
    {
        Id = Guid.NewGuid();
        Type = type;
        Payload = payload;
        OccurredOnUtc = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }

    public string Type { get; private set; } = string.Empty;

    public string Payload { get; private set; } = string.Empty;

    public DateTime OccurredOnUtc { get; private set; }

    public DateTime? ProcessedOnUtc { get; private set; }

    public int Attempts { get; private set; }

    public string? LastError { get; private set; }

    public DateTime? NextAttemptOnUtc { get; private set; }

    public bool IsProcessed =>
        ProcessedOnUtc.HasValue;

    public void MarkAsProcessed()
    {
        ProcessedOnUtc = DateTime.UtcNow;
        LastError = null;
        NextAttemptOnUtc = null;
    }

    public void RegisterFailure(
        string error)
    {
        Attempts++;
        LastError = error;
        NextAttemptOnUtc = DateTime.UtcNow.AddSeconds(
            Math.Min(Attempts * 5, 60));
    }
}