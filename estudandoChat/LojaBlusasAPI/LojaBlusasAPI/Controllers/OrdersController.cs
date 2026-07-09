using LojaBlusasAPI.Data;
using LojaBlusasAPI.Models;
using LojaBlusasAPI.Models.DTOs;
using LojaBlusasAPI.Models.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LojaBlusasAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrdersController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/<OrdersController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> Get()
        {
            var orders = await _context.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .OrderByDescending(o => o.CreateAt)
                .ToListAsync();
            return Ok(orders);
        }

        // GET api/<OrdersController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetById(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            if(order == null)
            {
                return NotFound();
            }
            return Ok(order);
        }

        // POST api/<OrdersController>
        [HttpPost]
        public async Task<ActionResult<Order>> Post([FromBody] CreateOrderDto dto)
        {
            decimal totalCalculado = 0;

            var order = new Order
            {
                CustomerName = dto.CustomerName,
                CustomerEmail = dto.CustomerEmail,
                CustomerPhone = dto.CustomerPhone,
                Address = dto.Address,
                Items = new List<OrderItem>()
            };

            foreach (var itemDto in dto.Items)
            {
                var product = await _context.Products.FindAsync(itemDto.ProductId);

                if(product == null)
                {
                    return BadRequest($"Produto {itemDto.ProductId} não encontrado!");
                }
                if(product.Stock < itemDto.Quantity)
                {
                    return BadRequest($"Estoque insuficiente para {product.Name}! " +$"Disponivel: {product.Stock}");
                }

                totalCalculado += product.Price * itemDto.Quantity;

                var orderItem = new OrderItem
                {
                    ProductId = itemDto.ProductId,
                    Quantity = itemDto.Quantity,
                    UnitPrice = product.Price
                };

                product.Stock -= itemDto.Quantity;
                order.Items.Add(orderItem);
            }

            order.Total = totalCalculado;

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return Ok(order);
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> Put(int id, [FromBody] UpdateOrderStatusDto dto)
        {
            var order = await _context.Orders.FindAsync(id);
            if(order == null)
            {
                return NotFound("Pedido não encontrado!");
            }

            if(!Enum.IsDefined(typeof(OrderStatus), dto.Status))
            {
                return BadRequest("Status inválido.");
            }

            order.Status = dto.Status;
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Status atualizado com sucesso.",
                data = order
            });
        }
    }
}
