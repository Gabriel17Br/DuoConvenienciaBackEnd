namespace DuoBackEnd.DTO;

public class ItemPedidoDTO
{
    public string NomeItem { get; set; } = string.Empty;
    public int Quantidade { get; set; }
    public decimal PrecoUnitario { get; set; }
    public int IdProduto { get; set; }
}