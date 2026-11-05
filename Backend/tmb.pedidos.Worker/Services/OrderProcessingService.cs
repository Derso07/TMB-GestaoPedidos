using tmb.pedidos.Domain.Enum;
using tmb.pedidos.Domain.Interfaces;

namespace tmb.pedidos.Worker;

public class OrderProcessingService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<OrderProcessingService> _logger;

    public OrderProcessingService(
        IServiceScopeFactory scopeFactory,
        ILogger<OrderProcessingService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public async Task ProcessAsync(
        Guid orderId,
        CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();

        var orderRepository =
            scope.ServiceProvider
                .GetRequiredService<IOrderService>();

        var order = await orderRepository.GetById(
            orderId,
            cancellationToken);

        if (order is null)
        {
            _logger.LogWarning(
                "Pedido {OrderId} não encontrado.",
                orderId);

            return;
        }

        // Idempotência básica
        if (order.OrderStatus != OrderStatus.Pending)
        {
            _logger.LogInformation(
                "Pedido {OrderId} já foi processado ou está em processamento. Status atual: {Status}",
                orderId,
                order.OrderStatus);

            return;
        }

        order.OrderStatus = OrderStatus.Processing;

        await orderRepository.Update(
            order,
            cancellationToken);

        _logger.LogInformation(
            "Pedido {OrderId} alterado para Processando.",
            orderId);

        await Task.Delay(
            TimeSpan.FromSeconds(5),
            cancellationToken);

        order.OrderStatus = OrderStatus.Completed;

        await orderRepository.Update(
            order,
            cancellationToken);

        _logger.LogInformation(
            "Pedido {OrderId} alterado para Finalizado.",
            orderId);
    }
}