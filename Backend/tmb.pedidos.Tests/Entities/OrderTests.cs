using tmb.pedidos.Domain.Entities;
using tmb.pedidos.Domain.Enum;

namespace tmb.pedidos.Tests.Entities;

public class OrderTests
{
    [Fact]
    public void DeveCriarPedidoComDadosInformados()
    {
        // Arrange
        const string customer = "João da Silva";
        const string product = "Notebook";
        const decimal value = 3500m;

        // Act
        var order = new Order(
            customer,
            product,
            value);

        // Assert
        Assert.NotEqual(Guid.Empty, order.Id);
        Assert.Equal(customer, order.Customer);
        Assert.Equal(product, order.Product);
        Assert.Equal(value, order.Value);
    }

    [Fact]
    public void DeveCriarPedidoComStatusPending()
    {
        // Arrange and Act
        var order = new Order(
            "João da Silva",
            "Notebook",
            3500m);

        // Assert
        Assert.Equal(
            OrderStatus.Pending,
            order.OrderStatus);
    }

    [Fact]
    public void DeveCriarDataDeCriacao()
    {
        // Arrange
        var before = DateTime.UtcNow;

        // Act
        var order = new Order(
            "João da Silva",
            "Notebook",
            3500m);

        var after = DateTime.UtcNow;

        // Assert
        Assert.InRange(
            order.CreatOn,
            before,
            after);
    }

    [Fact]
    public void DeveCriarHistoricoInicialComStatusPending()
    {
        // Arrange and Act
        var order = new Order(
            "João da Silva",
            "Notebook",
            3500m);

        // Assert
        Assert.NotEmpty(order.StatusHistory);

        var history = order.StatusHistory
            .Single();

        Assert.Null(history.PreviousStatus);

        Assert.Equal(
            OrderStatus.Pending,
            history.NewStatus);
    }

    [Fact]
    public void DeveAlterarStatusDePendingParaProcessing()
    {
        // Arrange
        var order = new Order(
            "João da Silva",
            "Notebook",
            3500m);

        // Act
        order.ChangeStatus(
            OrderStatus.Processing);

        // Assert
        Assert.Equal(
            OrderStatus.Processing,
            order.OrderStatus);
    }

    [Fact]
    public void DeveAlterarStatusDeProcessingParaCompleted()
    {
        // Arrange
        var order = new Order(
            "João da Silva",
            "Notebook",
            3500m);

        order.ChangeStatus(
            OrderStatus.Processing);

        // Act
        order.ChangeStatus(
            OrderStatus.Completed);

        // Assert
        Assert.Equal(
            OrderStatus.Completed,
            order.OrderStatus);
    }

    [Fact]
    public void DeveRegistrarHistoricoAoAlterarStatus()
    {
        // Arrange
        var order = new Order(
            "João da Silva",
            "Notebook",
            3500m);

        // Act
        order.ChangeStatus(
            OrderStatus.Processing);

        order.ChangeStatus(
            OrderStatus.Completed);

        // Assert
        Assert.Equal(
            3,
            order.StatusHistory.Count);

        var history = order.StatusHistory
            .OrderBy(item => item.ChangeAt)
            .ToList();

        Assert.Null(history[0].PreviousStatus);
        Assert.Equal(
            OrderStatus.Pending,
            history[0].NewStatus);

        Assert.Equal(
            OrderStatus.Pending,
            history[1].PreviousStatus);

        Assert.Equal(
            OrderStatus.Processing,
            history[1].NewStatus);

        Assert.Equal(
            OrderStatus.Processing,
            history[2].PreviousStatus);

        Assert.Equal(
            OrderStatus.Completed,
            history[2].NewStatus);
    }

    [Fact]
    public void NaoDeveCriarHistoricoAoManterMesmoStatus()
    {
        // Arrange
        var order = new Order(
            "João da Silva",
            "Notebook",
            3500m);

        var historyCountBefore =
            order.StatusHistory.Count;

        // Act
        order.ChangeStatus(
            OrderStatus.Pending);

        // Assert
        Assert.Equal(
            OrderStatus.Pending,
            order.OrderStatus);

        Assert.Equal(
            historyCountBefore,
            order.StatusHistory.Count);
    }
    
    [Fact]
    public void DeveRegistrarHistoricoDasTransicoes()
    {
        // Arrange
        var order = new Order(
            "João da Silva",
            "Notebook",
            3500m);

        // Act
        order.ChangeStatus(OrderStatus.Processing);
        order.ChangeStatus(OrderStatus.Completed);

        // Assert
        Assert.Equal(
            3,
            order.StatusHistory.Count);

        var history = order.StatusHistory
            .ToList();

        Assert.Null(history[0].PreviousStatus);
        Assert.Equal(
            OrderStatus.Pending,
            history[0].NewStatus);

        Assert.Equal(
            OrderStatus.Pending,
            history[1].PreviousStatus);

        Assert.Equal(
            OrderStatus.Processing,
            history[1].NewStatus);

        Assert.Equal(
            OrderStatus.Processing,
            history[2].PreviousStatus);

        Assert.Equal(
            OrderStatus.Completed,
            history[2].NewStatus);
    }
    
    [Fact]
    public void NaoDevePermitirAlterarDePendingParaCompleted()
    {
        // Arrange
        var order = new Order(
            "João da Silva",
            "Notebook",
            3500m);

        // Act and Assert
        Assert.Throws<InvalidOperationException>(() =>
            order.ChangeStatus(OrderStatus.Completed));
    }
}
