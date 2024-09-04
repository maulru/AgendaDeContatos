using Core.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Repository.Configurations
{
    public class EstadoConfiguration : IEntityTypeConfiguration<Estado>
    {
        public void Configure(EntityTypeBuilder<Estado> builder)
        {
            builder.ToTable("Estado");

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).HasColumnType("INT").UseIdentityColumn();
            builder.Property(e => e.Nome).HasColumnName("Nome").HasColumnType("VARCHAR(100)").IsRequired();
        }
    }
}
