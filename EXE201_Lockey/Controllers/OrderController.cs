using EXE201_Lockey.Dto;
using EXE201_Lockey.Services;
using Microsoft.AspNetCore.Mvc;

namespace EXE201_Lockey.Controllers
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

        // GET: api/Orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDTO>>> GetOrders()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return Ok(orders);
        }

        // GET: api/Orders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDTO>> GetOrder(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
            {
                return NotFound("Order not found.");
            }

            return Ok(order);
        }

        // POST: api/Orders
        [HttpPost]
        public async Task<ActionResult> PostOrder(OrderDTO orderDTO)
        {
            var message = await _orderService.AddOrderAsync(orderDTO);
            if (message.Contains("Error"))
            {
                return BadRequest(message);
            }

            return Ok(message);
        }

        // PUT: api/Orders/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrder(int id, OrderDTO orderDTO)
        {
            var message = await _orderService.UpdateOrderAsync(id, orderDTO);
            if (message == "Order not found.")
            {
                return NotFound(message);
            }
            if (message.Contains("Error"))
            {
                return BadRequest(message);
            }

            return Ok(message);
        }

        // DELETE: api/Orders/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var message = await _orderService.DeleteOrderAsync(id);
            if (message == "Order not found.")
            {
                return NotFound(message);
            }
            if (message.Contains("Error"))
            {
                return BadRequest(message);
            }

            return Ok(message);
        }
    }


}
