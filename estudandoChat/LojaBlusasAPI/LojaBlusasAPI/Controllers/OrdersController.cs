using LojaBlusasAPI.Data;
using LojaBlusasAPI.Models;
using LojaBlusasAPI.Models.DTOs;
using LojaBlusasAPI.Models.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

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

        /// <summary>
        /// Lista todos os pedidos cadastrados.
        /// </summary>
        /// <returns>Lista de pedidos, com itens e produtos incluídos.</returns>
        /// <response code="200">Retorna a lista de pedidos.</response>
        /// <response code="401">Requisição sem token ou token inválido.</response>
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Order>>> Get()
        {
            var orders = await _context.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .OrderByDescending(o => o.CreateAt)
                .ToListAsync();
            return Ok(orders);
        }

        /// <summary>
        /// Busca um pedido específico pelo ID.
        /// </summary>
        /// <param name="id">ID do pedido.</param>
        /// <returns>Dados completos do pedido, incluindo itens e produtos.</returns>
        /// <response code="200">Pedido encontrado.</response>
        /// <response code="401">Requisição sem token ou token inválido.</response>
        /// <response code="404">Pedido não encontrado.</response>
        [HttpGet("{id}")]
        [Authorize]
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

        /// <summary>
        /// Cria um novo pedido.
        /// </summary>
        /// <param name="dto">Dados do cliente e itens do pedido.</param>
        /// <returns>Pedido criado, com total calculado pelo servidor.</returns>
        /// <response code="200">Pedido criado com sucesso.</response>
        /// <response code="400">Produto não encontrado ou estoque insuficiente.</response>
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

        /// <summary>
        /// Atualiza o status de um pedido (ex: Pendente, Pago, Enviado).
        /// </summary>
        /// <param name="id">ID do pedido.</param>
        /// <param name="dto">Novo status do pedido.</param>
        /// <returns>Confirmação da atualização.</returns>
        /// <response code="200">Status atualizado com sucesso.</response>
        /// <response code="400">Status inválido.</response>
        /// <response code="401">Requisição sem token ou token inválido.</response>
        /// <response code="404">Pedido não encontrado.</response>
        [HttpPut("{id}")]
        [Authorize]
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
