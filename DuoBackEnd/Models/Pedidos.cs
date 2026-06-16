using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DuoBackEnd.Models
{
    public class Pedidos
    {
        [Key]
        [Column("id_pedido")]
        public int IdPedido {get;set;}
        [Column("data_pedido")]
        public DateTime DtPedido {get;set;}
        [Column("valor_total")]
        public decimal ValorTotal {get;set;}
        [Column("id_usuario")]
        public int IdUsuario {get;set;}
    }
}