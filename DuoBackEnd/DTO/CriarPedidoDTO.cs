using System.Collections.Generic;

namespace DuoBackEnd.DTO;

public class CriarPedidoDTO
{
    // Quem está fazendo o pedido
    public int IdUsuario { get; set; }

    // A lista de produtos que essa pessoa está comprando
    // Vinculamos com a outra classe 'ItemPedidoDTO' que você já tem criada
    public List<ItemPedidoDTO> Itens { get; set; } = new List<ItemPedidoDTO>();
}