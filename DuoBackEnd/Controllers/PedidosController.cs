using DuoBackEnd.DTO;
using DuoBackEnd.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DuoBackEnd.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PedidosController : ControllerBase
{
    private readonly AppDbContext _context;

    public PedidosController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> CriarPedido([FromBody] CriarPedidoDTO dto)
    {
        // Validação inicial
        if (dto == null || dto.Itens == null)
        {
            return BadRequest(new { erro = "O usuário e pelo menos um item são obrigatórios." });
        }

        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // 1. Calcula o valor total usando a lista do DTO correto
            decimal valorTotal = 0;
            foreach (var item in dto.Itens)
            {
                valorTotal += item.Quantidade * item.PrecoUnitario;
            }

            // 2. Cria e preenche a entidade Pedidos
            var novoPedido = new Pedidos
            {
                DtPedido = DateTime.Now,
                ValorTotal = valorTotal,
                IdUsuario = dto.IdUsuario
            };

            _context.Pedidos.Add(novoPedido);
            await _context.SaveChangesAsync(); // Salva para gerar o id_pedido

            // 3. Varre a lista vinda do DTO (Corrigido para dto.Itens)
            foreach (var itemDTO in dto.Itens)
            {
                var novoItem = new Itens
                {
                    NomeItem = itemDTO.NomeItem,
                    Quantidade = itemDTO.Quantidade,
                    PrecoUnitario = itemDTO.PrecoUnitario, // Agora vai bater com o Model ajustado
                    IdPedido = novoPedido.IdPedido 
                };

                _context.Itens.Add(novoItem);
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return StatusCode(201, new
            {
                mensagem = "Pedido criado com sucesso!",
                id_pedido = novoPedido.IdPedido,
                valor_total = novoPedido.ValorTotal
            });
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return StatusCode(500, new { erro = "Erro interno ao processar o pedido.", detalhes = ex.Message });
        }
    }
}