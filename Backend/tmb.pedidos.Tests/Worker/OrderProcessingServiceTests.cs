using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using tmb.pedidos.Domain.Entities;
using tmb.pedidos.Domain.Enum;
using tmb.pedidos.Domain.Interfaces;
using tmb.pedidos.Worker;

namespace tmb.pedidos.Tests.Worker;

public class OrderProcessingServiceTests
{
    [Fact]
    public async Task DeveProcessarPedidoPendenteAteCompleted()
    {
        // Arrange
        var order = new Order(
            "João da Silva",
            "Notebook",
            3500m);

        var orderId = order.Id;

        var orderServiceMock =
            new Mock<IOrderService>();

        orderServiceMock
            .SetupSequence(service =>
                service.GetById(
                    orderId,
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(order)
            .ReturnsAsync(order);

        var statusesUpdated =
            new List<OrderStatus>();

        orderServiceMock
            .Setup(service =>
                service.Update(
                    It.IsAny<Order>(),
                    It.IsAny<CancellationToken>()))
            .Callback<Order, CancellationToken>(
                (updatedOrder, _) =>
                {
                    statusesUpdated.Add(
                        updatedOrder.OrderStatus);
                })
            .Returns(Task.CompletedTask);

        var firstScope =
            CreateScope(orderServiceMock);

        var secondScope =
            CreateScope(orderServiceMock);

        var scopeFactoryMock =
            new Mock<IServiceScopeFactory>();

        scopeFactoryMock
            .SetupSequence(factory =>
                factory.CreateScope())
            .Returns(firstScope.Object)
            .Returns(secondScope.Object);

        var service = new OrderProcessingService(
            scopeFactoryMock.Object,
            NullLogger<OrderProcessingService>.Instance);

        // Act
        await service.ProcessAsync(
            orderId,
            CancellationToken.None);

        // Assert
        Assert.Equal(
            OrderStatus.Completed,
            order.OrderStatus);

        Assert.Equal(
            new[]
            {
                OrderStatus.Processing,
                OrderStatus.Completed
            },
            statusesUpdated);

        orderServiceMock.Verify(
            repository =>
                repository.GetById(
                    orderId,
                    It.IsAny<CancellationToken>()),
            Times.Exactly(2));

        orderServiceMock.Verify(
            repository =>
                repository.Update(
                    It.IsAny<Order>(),
                    It.IsAny<CancellationToken>()),
            Times.Exactly(2));

        scopeFactoryMock.Verify(
            factory =>
                factory.CreateScope(),
            Times.Exactly(2));
    }

    [Fact]
    public async Task NaoDeveAtualizarPedidoInexistente()
    {
        // Arrange
        var orderId = Guid.NewGuid();

        var orderServiceMock =
            new Mock<IOrderService>();

        orderServiceMock
            .Setup(service =>
                service.GetById(
                    orderId,
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync((Order?)null);

        var scope =
            CreateScope(orderServiceMock);

        var scopeFactoryMock =
            new Mock<IServiceScopeFactory>();

        scopeFactoryMock
            .Setup(factory =>
                factory.CreateScope())
            .Returns(scope.Object);

        var service = new OrderProcessingService(
            scopeFactoryMock.Object,
            NullLogger<OrderProcessingService>.Instance);

        // Act
        await service.ProcessAsync(
            orderId,
            CancellationToken.None);

        // Assert
        orderServiceMock.Verify(
            repository =>
                repository.GetById(
                    orderId,
                    It.IsAny<CancellationToken>()),
            Times.Once);

        orderServiceMock.Verify(
            repository =>
                repository.Update(
                    It.IsAny<Order>(),
                    It.IsAny<CancellationToken>()),
            Times.Never);

        scopeFactoryMock.Verify(
            factory =>
                factory.CreateScope(),
            Times.Once);
    }

    [Fact]
    public async Task NaoDeveProcessarPedidoQueJaEstaProcessing()
    {
        // Arrange
        var order = new Order(
            "João da Silva",
            "Notebook",
            3500m);

        order.ChangeStatus(
            OrderStatus.Processing);

        var orderId = order.Id;

        var orderServiceMock =
            new Mock<IOrderService>();

        orderServiceMock
            .Setup(service =>
                service.GetById(
                    orderId,
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        var scope =
            CreateScope(orderServiceMock);

        var scopeFactoryMock =
            new Mock<IServiceScopeFactory>();

        scopeFactoryMock
            .Setup(factory =>
                factory.CreateScope())
            .Returns(scope.Object);

        var service = new OrderProcessingService(
            scopeFactoryMock.Object,
            NullLogger<OrderProcessingService>.Instance);

        // Act
        await service.ProcessAsync(
            orderId,
            CancellationToken.None);

        // Assert
        Assert.Equal(
            OrderStatus.Processing,
            order.OrderStatus);

        orderServiceMock.Verify(
            repository =>
                repository.Update(
                    It.IsAny<Order>(),
                    It.IsAny<CancellationToken>()),
            Times.Never);

        scopeFactoryMock.Verify(
            factory =>
                factory.CreateScope(),
            Times.Once);
    }

    [Fact]
    public async Task NaoDeveProcessarPedidoQueJaEstaCompleted()
    {
        // Arrange
        var order = new Order(
            "João da Silva",
            "Notebook",
            3500m);

        order.ChangeStatus(
            OrderStatus.Processing);

        order.ChangeStatus(
            OrderStatus.Completed);

        var orderId = order.Id;

        var orderServiceMock =
            new Mock<IOrderService>();

        orderServiceMock
            .Setup(service =>
                service.GetById(
                    orderId,
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        var scope =
            CreateScope(orderServiceMock);

        var scopeFactoryMock =
            new Mock<IServiceScopeFactory>();

        scopeFactoryMock
            .Setup(factory =>
                factory.CreateScope())
            .Returns(scope.Object);

        var service = new OrderProcessingService(
            scopeFactoryMock.Object,
            NullLogger<OrderProcessingService>.Instance);

        // Act
        await service.ProcessAsync(
            orderId,
            CancellationToken.None);

        // Assert
        Assert.Equal(
            OrderStatus.Completed,
            order.OrderStatus);

        orderServiceMock.Verify(
            repository =>
                repository.Update(
                    It.IsAny<Order>(),
                    It.IsAny<CancellationToken>()),
            Times.Never);

        scopeFactoryMock.Verify(
            factory =>
                factory.CreateScope(),
            Times.Once);
    }

    private static Mock<IServiceScope> CreateScope(
        Mock<IOrderService> orderServiceMock)
    {
        var serviceProviderMock =
            new Mock<IServiceProvider>();

        serviceProviderMock
            .Setup(provider =>
                provider.GetService(
                    typeof(IOrderService)))
            .Returns(orderServiceMock.Object);

        var scopeMock =
            new Mock<IServiceScope>();

        scopeMock
            .SetupGet(scope =>
                scope.ServiceProvider)
            .Returns(serviceProviderMock.Object);

        return scopeMock;
    }
}
