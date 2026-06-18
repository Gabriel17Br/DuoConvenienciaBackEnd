using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DuoBackEnd.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DuoBackEnd.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProdutosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProdutosController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("ListarProdutos")]
         public async Task<ActionResult<IEnumerable<Produtos>>> ListarProdutos()
        {
            var produtos = await _context.Produtos.ToListAsync();
            return Ok(produtos);
        }

    }
}