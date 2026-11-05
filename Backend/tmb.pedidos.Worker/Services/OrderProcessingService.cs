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
        cancellationToken.ThrowIfCancellationRequested();

        _logger.LogInformation(
            "Iniciando processamento do pedido {OrderId}. Token cancelado: {IsCancellationRequested}",
            orderId,
            cancellationToken.IsCancellationRequested);

        // Primeiro escopo: buscar e mudar para Processing
        using (var scope = _scopeFactory.CreateScope())
        {
            var orderRepository = scope.ServiceProvider
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

            if (order.OrderStatus != OrderStatus.Pending)
            {
                _logger.LogInformation(
                    "Pedido {OrderId} ignorado. Status atual: {Status}",
                    orderId,
                    order.OrderStatus);

                return;
            }

            order.ChangeStatus(OrderStatus.Processing);

            await orderRepository.Update(
                order,
                cancellationToken);

            _logger.LogInformation(
                "Pedido {OrderId} alterado para Processing.",
                orderId);
        }

        // Simula o processamento assíncrono
        await Task.Delay(
            TimeSpan.FromSeconds(5),
            cancellationToken);

        // Segundo escopo: buscar novamente e mudar para Completed
        using (var scope = _scopeFactory.CreateScope())
        {
            var orderRepository = scope.ServiceProvider
                .GetRequiredService<IOrderService>();

            var order = await orderRepository.GetById(
                orderId,
                cancellationToken);

            if (order is null)
            {
                _logger.LogWarning(
                    "Pedido {OrderId} não encontrado ao finalizar.",
                    orderId);

                return;
            }

            if (order.OrderStatus != OrderStatus.Processing)
            {
                _logger.LogInformation(
                    "Pedido {OrderId} não pode ser finalizado. Status atual: {Status}",
                    orderId,
                    order.OrderStatus);

                return;
            }

            order.ChangeStatus(OrderStatus.Completed);

            await orderRepository.Update(
                order,
                cancellationToken);

            _logger.LogInformation(
                "Pedido {OrderId} alterado para Completed.",
                orderId);
        }
    }
}
