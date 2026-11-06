using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using tmb.pedidos.Domain.Interfaces.MessagingPublish;
using tmb.pedidos.Domain.Messaging;
using tmb.pedidos.Infrastructure.Outbox;

namespace tmb.pedidos.Worker.Outbox;

public sealed class OutboxPublisher : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<OutboxPublisher> _logger;

    public OutboxPublisher(
        IServiceScopeFactory scopeFactory,
        ILogger<OutboxPublisher> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "Outbox Publisher iniciado.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await PublishPendingMessagesAsync(
                    stoppingToken);
            }
            catch (OperationCanceledException)
                when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception exception)
            {
                _logger.LogError(
                    exception,
                    "Erro ao processar mensagens do Outbox.");
            }

            await Task.Delay(
                TimeSpan.FromSeconds(2),
                stoppingToken);
        }

        _logger.LogInformation(
            "Outbox Publisher finalizado.");
    }

    private async Task PublishPendingMessagesAsync(
        CancellationToken cancellationToken)
    {
        using var scope =
            _scopeFactory.CreateScope();

        var outboxRepository =
            scope.ServiceProvider
                .GetRequiredService<IOutboxRepository>();

        var messagePublisher =
            scope.ServiceProvider
                .GetRequiredService<IMessagePublisher>();

        _logger.LogInformation(
            "Consultando mensagens Outbox pendentes.");

        var messages =
            await outboxRepository.GetPendingAsync(
                batchSize: 20,
                cancellationToken);

        _logger.LogInformation(
            "Mensagens Outbox encontradas: {Count}.",
            messages.Count);

        foreach (var outboxMessage in messages)
        {
            try
            {
                await PublishMessageAsync(
                    outboxMessage,
                    messagePublisher,
                    cancellationToken);

                outboxMessage.MarkAsProcessed();

                await outboxRepository.SaveChangesAsync(
                    cancellationToken);

                _logger.LogInformation(
                    "Mensagem Outbox {MessageId} publicada e marcada como processada.",
                    outboxMessage.Id);
            }
            catch (Exception exception)
            {
                outboxMessage.RegisterFailure(
                    exception.Message);

                await outboxRepository.SaveChangesAsync(
                    cancellationToken);

                _logger.LogError(
                    exception,
                    "Erro ao publicar a mensagem Outbox {MessageId}. Tentativas: {Attempts}.",
                    outboxMessage.Id,
                    outboxMessage.Attempts);
            }
        }
    }

    private static async Task PublishMessageAsync(
        OutboxMessage outboxMessage,
        IMessagePublisher messagePublisher,
        CancellationToken cancellationToken)
    {
        if (!string.Equals(
                outboxMessage.Type,
                "OrderCreated",
                StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                $"Tipo de evento não suportado: {outboxMessage.Type}.");
        }

        var message =
            JsonSerializer.Deserialize<OrderCreatedMessage>(
                outboxMessage.Payload);

        if (message is null)
        {
            throw new InvalidOperationException(
                $"Não foi possível desserializar o OutboxMessage {outboxMessage.Id}.");
        }

        await messagePublisher.PublishOrderCreatedAsync(
            message,
            cancellationToken);
    }
}
