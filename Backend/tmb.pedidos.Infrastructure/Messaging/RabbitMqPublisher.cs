using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using tmb.pedidos.Domain.Interfaces.MessagingPublish;
using tmb.pedidos.Domain.Messaging;

namespace tmb.pedidos.Infrastructure.Messaging;

public class RabbitMqPublisher : IMessagePublisher
{
    private const string ExchangeName = "orders.exchange";
    private const string RoutingKey = "order.created";

    private readonly IConfiguration _configuration;

    public RabbitMqPublisher(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public Task PublishOrderCreatedAsync(
        OrderCreatedMessage message,
        CancellationToken cancellationToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = _configuration["RabbitMQ:Host"] ?? "localhost",
            Port = int.Parse(_configuration["RabbitMQ:Port"] ?? "5672"),
            UserName = _configuration["RabbitMQ:Username"] ?? "guest",
            Password = _configuration["RabbitMQ:Password"] ?? "guest"
        };

        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.ExchangeDeclare(
            exchange: ExchangeName,
            type: ExchangeType.Direct,
            durable: true,
            autoDelete: false);

        var json = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(json);

        var properties = channel.CreateBasicProperties();
        properties.Persistent = true;
        properties.MessageId = message.OrderId.ToString();
        properties.CorrelationId = message.CorrelationId.ToString();
        properties.Type = message.EventType;
        properties.ContentType = "application/json";

        channel.BasicPublish(
            exchange: ExchangeName,
            routingKey: RoutingKey,
            basicProperties: properties,
            body: body);

        return Task.CompletedTask;
    }
}