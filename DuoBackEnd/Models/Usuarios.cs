using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DuoBackEnd.Models
{
    public class Usuarios
    {
        [Key]
        [Column("id_usuario")]
        public int IdUsuario { get; set; }
        [Column("nome")]
        public string Nome {get;set;} = string.Empty;
        [Column("email")]
        public string Email {get;set;} = string.Empty;
        [Column("telefone")]
        public string Telefone {get;set;} = string.Empty;
        [Column("senha")]
        public string Senha {get;set;} = string.Empty;
    }
}