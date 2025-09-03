using LojaNova.Ecommerce.Api.Services;
using LojaNova.Shared.Models;
using Microsoft.AspNetCore.Mvc;

namespace LojaNova.Ecommerce.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return Ok(orders);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            return Ok(order);
        }

        [HttpPost]
        public async Task<ActionResult> PlaceOrder(Order order)
        {
            try
            {
                // A API envia o pedido para a fila. O processamento real acontece na Azure Function.
                await _orderService.SendOrderToQueueAsync(order);
                // Retorna Accepted (202) para indicar que a requisição foi aceita para processamento
                return Accepted("Order has been received and will be processed shortly.");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                // Logar a exceção
                return StatusCode(500, $"An error occurred while placing the order: {ex.Message}");
            }
        }
    }
}
