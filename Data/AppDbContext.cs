using InventarioAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace InventarioAPI.Data
{
    /// <summary>
    /// Contexto do Entity Framework que gerencia a sessão
    /// com o banco de dados e mapeia as entidades.
    /// </summary>
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        /// <summary>
        /// Tabela de Ativos no banco de dados.
        /// </summary>
        public DbSet<Ativo> Ativos { get; set; }

        /// <summary>
        /// Tabela de Usuários no banco de dados.
        /// </summary>
        public DbSet<Usuario> Usuarios { get; set; }
    }
}