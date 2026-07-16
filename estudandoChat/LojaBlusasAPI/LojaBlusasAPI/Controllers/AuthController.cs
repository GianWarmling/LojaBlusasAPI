using LojaBlusasAPI.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LojaBlusasAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        
        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Realiza o login do administrador e retorna um token JWT.
        /// </summary>
        /// <param name="dto">Email e senha do administrador.</param>
        /// <returns>Token JWT válido por 2 horas.</returns>
        /// <response code="200">Login realizado com sucesso, retorna o token.</response>
        /// <response code="401">Email ou senha inválidos.</response>
        [HttpPost("login")]
        public IActionResult Post([FromBody] LoginDto dto)
        {
            if(dto.Email != "admin@lojablusas.com" || dto.Password != "123456")
            {
                return Unauthorized("Usuário ou senha inválida!");
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, dto.Email)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: credentials
                );

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token)
            });
        }
    }
}
