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

     [HttpGet("ListarPedidos")]
     public async Task<ActionResult<IEnumerable<Pedidos>>> ListarPedido()
    {
        var pedidos = await _context.Pedidos.ToListAsync();
        return Ok(pedidos);
    }

   [HttpPost]
public async Task<IActionResult> CriarPedido([FromBody] CriarPedidoDTO dto)
{
    if (dto == null || dto.Itens == null || !dto.Itens.Any())
    {
        return BadRequest(new { erro = "O usuário e pelo menos um item são obrigatórios." });
    }

    using var transaction = await _context.Database.BeginTransactionAsync();

    try
    {
        decimal valorTotal = 0;

        // 1. Cria o Pedido base
        var novoPedido = new Pedidos
        {
            DtPedido = DateTime.Now,
            IdUsuario = dto.IdUsuario,
            ValorTotal = 0 // Calculado abaixo
        };

        _context.Pedidos.Add(novoPedido);
        await _context.SaveChangesAsync(); 

        // 2. Processa os itens: Valida estoque e subtrai
        foreach (var itemDTO in dto.Itens)
        {
            // Busca o produto no estoque (Produtos)
            var produtoNoEstoque = await _context.Produtos
                .FirstOrDefaultAsync(p => p.IdProduto == itemDTO.IdProduto);

            if (produtoNoEstoque == null)
            {
                throw new Exception($"Produto {itemDTO.NomeItem} não encontrado no estoque.");
            }

            if (produtoNoEstoque.QuantidadeEstoque < itemDTO.Quantidade)
            {
                throw new Exception($"Estoque insuficiente para o produto: {produtoNoEstoque.NomeProduto}.");
            }

            // Subtrai do estoque (Produtos)
            produtoNoEstoque.QuantidadeEstoque -= itemDTO.Quantidade;
            _context.Produtos.Update(produtoNoEstoque);

            // Adiciona ao carrinho (Itens)
            var novoItem = new Itens
            {
                NomeItem = itemDTO.NomeItem,
                Quantidade = itemDTO.Quantidade,
                PrecoUnitario = itemDTO.PrecoUnitario,
                IdPedido = novoPedido.IdPedido,
                IdProduto = itemDTO.IdProduto // Importante para ligar com o estoque
            };

            valorTotal += (itemDTO.Quantidade * itemDTO.PrecoUnitario);
            _context.Itens.Add(novoItem);
        }

        // 3. Finaliza o pedido
        novoPedido.ValorTotal = valorTotal;
        await _context.SaveChangesAsync();
        await transaction.CommitAsync();

        return StatusCode(201, new { mensagem = "Pedido criado e estoque atualizado!", id_pedido = novoPedido.IdPedido });
    }
    catch (Exception ex)
    {
        await transaction.RollbackAsync();
        return StatusCode(400, new { erro = "Erro ao criar pedido.", detalhes = ex.Message });
    }
}

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletarPedido(int id)
    {
    // 1. Inicia uma transação para garantir a consistência dos dados
    using var transaction = await _context.Database.BeginTransactionAsync();

    try
    {
        // 2. Busca o pedido incluindo os itens relacionados
        var pedido = await _context.Pedidos
            .Include(p => p.Itens)
            .FirstOrDefaultAsync(p => p.IdPedido == id);

        if (pedido == null)
        {
            return NotFound(new { erro = $"Pedido com ID {id} não encontrado." });
        }

        // 3. Devolve os itens para o estoque (Produtos)
        foreach (var item in pedido.Itens)
        {
            // Busca o produto correspondente no estoque
            var produtoNoEstoque = await _context.Produtos
                .FirstOrDefaultAsync(p => p.IdProduto == item.IdProduto);

            if (produtoNoEstoque != null)
            {
                // Soma a quantidade de volta ao estoque geral
                produtoNoEstoque.QuantidadeEstoque += item.Quantidade;
                _context.Produtos.Update(produtoNoEstoque);
            }
        }

        // 4. Remove os itens primeiro (por conta da restrição de chave estrangeira no banco)
        _context.Itens.RemoveRange(pedido.Itens);

        // 5. Remove o pedido
        _context.Pedidos.Remove(pedido);

        // 6. Salva todas as alterações e confirma a transação
        await _context.SaveChangesAsync();
        await transaction.CommitAsync();

        return Ok(new { mensagem = $"Pedido {id} deletado com sucesso e produtos devolvidos ao estoque!" });
    }
    catch (Exception ex)
    {
        // Se der qualquer erro, desfaz as alterações de estoque
        await transaction.RollbackAsync();
        return StatusCode(500, new { erro = "Erro ao deletar o pedido e estornar o estoque.", detalhes = ex.Message });
    }
}
}