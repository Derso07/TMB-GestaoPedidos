using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using tmb.pedidos.Domain.Messaging;

namespace tmb.pedidos.Worker;

public class OrderCreatedConsumer : BackgroundService
{
    private const string ExchangeName = "orders.exchange";
    private const string QueueName = "orders.order-created";
    private const string RoutingKey = "order.created";

    private readonly IConfiguration _configuration;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<OrderCreatedConsumer> _logger;

    private IConnection? _connection;
    private IModel? _channel;

    public OrderCreatedConsumer(
        IConfiguration configuration,
        IServiceScopeFactory scopeFactory,
        ILogger<OrderCreatedConsumer> logger)
    {
        _configuration = configuration;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = _configuration["RabbitMQ:Host"] ?? "localhost",
            Port = int.Parse(_configuration["RabbitMQ:Port"] ?? "5672"),
            UserName = _configuration["RabbitMQ:Username"] ?? "guest",
            Password = _configuration["RabbitMQ:Password"] ?? "guest",
            DispatchConsumersAsync = true
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.ExchangeDeclare(
            exchange: ExchangeName,
            type: ExchangeType.Direct,
            durable: true,
            autoDelete: false);

        _channel.QueueDeclare(
            queue: QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false);

        _channel.QueueBind(
            queue: QueueName,
            exchange: ExchangeName,
            routingKey: RoutingKey);

        _channel.BasicQos(
            prefetchSize: 0,
            prefetchCount: 1,
            global: false);

        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.Received += async (_, eventArgs) =>
        {
            await ConsumeMessageAsync(
                eventArgs,
                stoppingToken);
        };

        _channel.BasicConsume(
            queue: QueueName,
            autoAck: false,
            consumer: consumer);

        _logger.LogInformation(
            "Worker aguardando mensagens na fila {QueueName}.",
            QueueName);

        return Task.CompletedTask;
    }

    private async Task ConsumeMessageAsync(
        BasicDeliverEventArgs eventArgs,
        CancellationToken cancellationToken)
    {
        if (_channel is null)
        {
            return;
        }

        try
        {
            var json = Encoding.UTF8.GetString(
                eventArgs.Body.ToArray());

            var message = JsonSerializer.Deserialize<OrderCreatedMessage>(
                json);

            if (message is null)
            {
                throw new InvalidOperationException(
                    "Mensagem OrderCreated inválida.");
            }

            _logger.LogInformation(
                "Mensagem recebida. OrderId: {OrderId}, CorrelationId: {CorrelationId}, EventType: {EventType}",
                message.OrderId,
                message.CorrelationId,
                message.EventType);

            if (message.EventType != "OrderCreated")
            {
                throw new InvalidOperationException(
                    $"EventType não suportado: {message.EventType}");
            }

            using var scope = _scopeFactory.CreateScope();

            var processingService =
                scope.ServiceProvider
                    .GetRequiredService<OrderProcessingService>();

            await processingService.ProcessAsync(
                message.OrderId,
                cancellationToken);

            _channel.BasicAck(
                deliveryTag: eventArgs.DeliveryTag,
                multiple: false);
        }
        catch (OperationCanceledException)
            when (cancellationToken.IsCancellationRequested)
        {
            _logger.LogInformation(
                "Processamento cancelado porque o Worker está sendo encerrado.");
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Erro ao processar mensagem do pedido.");

            _channel.BasicNack(
                deliveryTag: eventArgs.DeliveryTag,
                multiple: false,
                requeue: true);
        }
    }

    public override void Dispose()
    {
        _channel?.Close();
        _connection?.Close();

        _channel?.Dispose();
        _connection?.Dispose();

        base.Dispose();
    }
}
