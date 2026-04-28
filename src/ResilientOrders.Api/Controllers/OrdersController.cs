using Microsoft.AspNetCore.Mvc;
using ResilientOrders.Api.Models;
using ResilientOrders.Api.Services;

namespace ResilientOrders.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Order>> Get()
    {
        return Ok(_orderService.ListOrders());
    }

    [HttpGet("{id:int}")]
    public ActionResult<Order> GetById(int id)
    {
        var order = _orderService.GetOrder(id);
        if (order is null)
        {
            return NotFound(new { message = $"Pedido {id} não encontrado." });
        }
        return Ok(order);
    }

    [HttpPost]
    public ActionResult<Order> Create([FromBody] CreateOrderRequest request)
    {
        if (request is null)
        {
            return BadRequest(new { message = "Payload inválido." });
        }

        try
        {
            var created = _orderService.PlaceOrder(request);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
