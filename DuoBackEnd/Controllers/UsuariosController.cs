using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DuoBackEnd.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace DuoBackEnd.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsuariosController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("Registro")]
        public async Task<ActionResult<Usuarios>> RegistroUsuario(DTO.NewUsuarioDTO newUsuario)
        {
            Usuarios usuario = new Usuarios
            {
                Nome = newUsuario.Nome,
                Telefone = newUsuario.Telefone,
                Email = newUsuario.Email,
                Senha = newUsuario.Senha // Dica futura: aplicar hash aqui depois!
            };

            await _context.Usuarios.AddAsync(usuario);
            await _context.SaveChangesAsync();
            return Ok(usuario);
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login(DTO.ValidacaoUsuario login)
        {
            // CORRIGIDO: Removido o .Include() que quebrava a consulta
            var UsuarioExist = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == login.Email);

            if (UsuarioExist == null)
            {
                return NotFound("Usuário não encontrado!!");
            }

            // CORRIGIDO: Trocado 'sb.ToString()' por 'login.Senha' para fins de validação direta
            if ((UsuarioExist.Senha ?? string.Empty) != login.Senha)
            {
                return BadRequest("Senha incorreta");
            }

            // Gerar o token JWT
            var claims = new[]
            {
                new Claim("id", UsuarioExist.IdUsuario.ToString()),
                new Claim(ClaimTypes.Name, UsuarioExist.Nome ?? string.Empty),
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes("f8A9xK2#pL0zQw7@Rm5TnY3uVb6C!dE1") // Garanta que essa chave tenha 32 caracteres
            );

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "apiFestaJulina",
                audience: "apiFestaJulina",
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            // Retorno de sucesso com o Token
            return Ok(new
            {
                idUsuario = UsuarioExist.IdUsuario,
                token = tokenString,
                usuario = UsuarioExist.Nome ?? string.Empty,
            });
        }

        [HttpPost("logout")]
        public ActionResult Logout()
        {
            // Não busca nada no banco. O Front-end chama essa rota e limpa o Token localmente.
            return Ok(new { mensagem = "Logout efetuado. Por favor, remova o token do cliente." });
        }
    }
}