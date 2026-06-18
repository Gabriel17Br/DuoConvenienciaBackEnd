using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DuoBackEnd.Models
{
    public class Produtos
    {
        [Key]
        [Column("id_produto")]
        public int IdProduto {get;set;}
        [Column("nome_produto")]
        public string NomeProduto {get;set;} = string.Empty;
        [Column("preco")]
        public decimal Preco {get;set;}
        [Column("quantidade")]
        public int Quantidade {get;set;}
    }
}