using Core.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Repository
{
    public class ApplicationDbContext : DbContext
    {
        #region Propriedades
        private readonly IConfiguration _connectionString;
        #endregion

        #region Construtores
        public ApplicationDbContext()
        {}

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IConfiguration connectionString) : base(options)
        {
            _connectionString = connectionString;
        }
        #endregion

        #region DbSets
        public DbSet<DDD> DDD { get; set; }
        #endregion

        #region Métodos Overrides
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
        #endregion
    }
}
