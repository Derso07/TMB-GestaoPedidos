using Microsoft.AspNetCore.Mvc;
using tmb.pedidos.API.Contracts.Orders;
using tmb.pedidos.API.Mappers;
using tmb.pedidos.API.Services.Orders;

namespace tmb.pedidos.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class OrderController : ControllerBase
{
    private readonly CreateOrderService _createOrderService;
    private readonly GetOrderByIdService  _getOrderByIdService;
    private readonly GetOrdersService   _getOrdersService;

    public OrderController(CreateOrderService createOrderService, 
        GetOrderByIdService getOrderByIdService, 
        GetOrdersService getOrdersService)
    {
        _createOrderService = createOrderService;
        _getOrderByIdService = getOrderByIdService;
        _getOrdersService = getOrdersService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody]OrderRequest orderRequest, 
        CancellationToken cancellationToken)
    {
        var order = await _createOrderService.CreateAsync(orderRequest.Customer, orderRequest.Product,
            orderRequest.Value, cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = order.Id },
            OrderMapper.ToResponse(order));
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var orders = await _getOrdersService.GetOrdersAsync(cancellationToken);
        
        return Ok(OrderMapper.ToResponse(orders));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id, 
        CancellationToken cancellationToken)
    {
        var order = await _getOrderByIdService.GetByIdAsync(id, cancellationToken);
        
        return Ok(order);
    }
}