using tmb.pedidos.Domain.Messaging;

namespace tmb.pedidos.Domain.Interfaces.MessagingPublish;

public interface IMessagePublisher
{
    public Task PublishOrderCreatedAsync(
        OrderCreatedMessage message,
        CancellationToken cancellationToken);
}