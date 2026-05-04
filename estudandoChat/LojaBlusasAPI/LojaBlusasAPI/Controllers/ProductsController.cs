using LojaBlusasAPI.Data;
using LojaBlusasAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LojaBlusasAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/<ProductsController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var products = await _context.Products.ToListAsync();
            return Ok(products);
        }

        // GET api/<ProductsController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var product = await _context.Products.FindAsync(id);
             if(product == null)
            {
                return NotFound(new
                { 
                    message = $"Produto com ID {id} não encontrado!"
                });
            }

            return Ok(product);
        }

        // POST api/<ProductsController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Product product)
        {
            if(product == null)
            {
                return BadRequest(new
                {
                    message = "Dados do produto inválidos!"
                });
            }

            if (string.IsNullOrWhiteSpace(product.Name))
            {
                return BadRequest(new
                {
                    message = "O nome do produto é obrigatório!"
                });
            }

            if(product.Price <= 0)
            {
                return BadRequest(new
                {
                    message = "O preço deve ser maior que zero!"
                });
            }

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = product.Id }, new
            {
                message = "Produto criado com sucesso.",
                data = product
            });
        }

        // PUT api/<ProductsController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Product product)
        {
            if (product == null)
            {
                return BadRequest(new
                {
                    message = "Dados do produto inválidos!"
                });
            }

            if (id != product.Id)
            {
                return BadRequest(new
                {
                    message = "O ID da URL é diferente do ID do produto!"
                });
            }

            if (string.IsNullOrWhiteSpace(product.Name))
            {
                return BadRequest(new
                {
                    message = "O nome do produto é obrigatório!"
                });
            }

            if (product.Price <= 0)
            {
                return BadRequest(new
                {
                    message = "O preço deve ser maior que zero!"
                });
            }

            var existingProduct = await _context.Products.FindAsync(id);

            if (existingProduct == null)
            {
                return NotFound(new
                {
                    message = $"Produto com ID {id} não encontrado!"
                });
            }

            existingProduct.Name = product.Name;
            existingProduct.Description = product.Description;
            existingProduct.Price = product.Price;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Produto atualizado com sucesso!",
                data = existingProduct
            });
        }

        // DELETE api/<ProductsController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if(product == null)
            {
                return NotFound(new
                {
                    message = $"Produto com ID {id} não encontrado!"
                });
            }

            _context.Products.Remove(product);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Produto removido com sucesso!",
                data = product
            });
        }
    }
}
