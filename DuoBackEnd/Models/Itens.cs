using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DuoBackEnd.Models;

[Table("itens")]
public class Itens
{
    [Key]
    [Column("id_item")]
    public int IdItem { get; set; }

    [Column("nome_item")]
    public string NomeItem { get; set; } = string.Empty;

    [Column("quantidade")]
    public int Quantidade { get; set; }

    [Column("preco_unitario")] // Nome da coluna no MySQL
    public decimal PrecoUnitario { get; set; } // Este nome precisa estar idêntico aqui!

    [Column("id_pedido")]
    public int IdPedido { get; set; }
}