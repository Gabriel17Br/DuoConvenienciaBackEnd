// Data/ApplicationDbContext.cs
using DuoBackEnd.Models;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }
    public DbSet<Usuarios> Usuarios { get; set; }
    public DbSet<Pedidos> Pedidos { get; set; }
    
    public DbSet<Produtos> Produtos { get; set; }
    public DbSet<Itens> Itens { get; set; }
}