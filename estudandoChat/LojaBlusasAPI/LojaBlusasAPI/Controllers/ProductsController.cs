using LojaBlusasAPI.Data;
using LojaBlusasAPI.Models;
using LojaBlusasAPI.Models.DTOs;
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

        /// <summary>
        /// Retorna todos os produtos cadastrados.
        /// </summary>
        /// <returns>Lista de Produtos.</returns>
        /// <response code="200">Produtos encontrados com sucesso.</response>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var products = await _context.Products.ToListAsync();
            return Ok(products);
        }

        /// <summary>
        /// Busca um produto pelo ID.
        /// </summary>
        /// <param name="id">ID do produto.</param>
        /// <returns>Dados do produto.</returns>
        /// <response code="200">Produto encontrado.</response>
        /// <response code="404">Produto não encontrado.</response>
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

        /// <summary>
        /// Cria um novo produto.
        /// </summary>
        /// <param name="dto">Dados para criação do produto.</param>
        /// <returns>Produto criado.</returns>
        /// <response code="201">Produto criado com sucesso.</response>
        /// <response code="400">Dados inválidos.</response>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateProductDto dto)
        {
            if(dto == null)
            {
                return BadRequest(new
                {
                    message = "Dados do produto inválidos!"
                });
            }

            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                return BadRequest(new
                {
                    message = "O nome do produto é obrigatório!"
                });
            }

            if(dto.Price <= 0)
            {
                return BadRequest(new
                {
                    message = "O preço deve ser maior que zero!"
                });
            }

            var product = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                ImageUrl = dto.ImageUrl,
                Size = dto.Size,
                Stock = dto.Stock
            };

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            var response = new ProductResponseDto
            {
                Id = product.Id,
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                ImageUrl = dto.ImageUrl
            };

            return CreatedAtAction(nameof(Get), new { id = product.Id }, new
            {
                message = "Produto criado com sucesso.",
                data = product
            });
        }

        /// <summary>
        /// Atualiza um produto existente.
        /// </summary>
        /// <param name="id">ID do produto.</param>
        /// <param name="dto">Dados atualizados.</param>
        /// <returns>Produto atualizado.</returns>
        /// <response code="200">Produto atualizadocom sucesso.</response>
        /// <response code="400">Dados inválidos.</response>
        /// <response code="404">Produto não encontrado.</response>
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] UpdateProductDto dto)
        {
            if (dto == null)
            {
                return BadRequest(new
                {
                    message = "Dados do produto inválidos!"
                });
            }

            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                return BadRequest(new
                {
                    message = "O nome do produto é obrigatório!"
                });
            }

            if (dto.Price <= 0)
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

            existingProduct.Name = dto.Name;
            existingProduct.Description = dto.Description;
            existingProduct.Price = dto.Price;
            existingProduct.ImageUrl = dto.ImageUrl;
            existingProduct.Size = dto.Size;
            existingProduct.Stock = dto.Stock;

            await _context.SaveChangesAsync();

            var response = new ProductResponseDto
            { 
                Id = existingProduct.Id,
                Name = existingProduct.Name,
                Description = existingProduct.Description,
                Price = existingProduct.Price,
                ImageUrl = existingProduct.ImageUrl
            };

            return Ok(new
            {
                message = "Produto atualizado com sucesso!",
                data = existingProduct
            });
        }

        /// <summary>
        /// Remove um produto pelo ID.
        /// </summary>
        /// <param name="id">ID do produto.</param>
        /// <returns>Produto removido.</returns>
        /// <response code="200">Produto removido com sucesso.</response>
        /// <response code="404">Produto não encontrado.</response>
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
