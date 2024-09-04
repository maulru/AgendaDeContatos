using Core.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Repository
{
    public class ApplicationDbContext : DbContext
    {
        private readonly IConfiguration _connectionString;

        public ApplicationDbContext()
        {

        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IConfiguration connectionString) :base(options)
        {
            _connectionString = connectionString;
        }

     

        public DbSet<Contato> Contato { get; set; }
        public DbSet<Telefone> Telefone { get; set; }
        public DbSet<DDD> DDD { get; set; }
        public DbSet<Regiao> Regiao { get; set; }
        public DbSet<Estado> Estado { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(_connectionString.GetConnectionString("ConnectionString"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }
    }
}
