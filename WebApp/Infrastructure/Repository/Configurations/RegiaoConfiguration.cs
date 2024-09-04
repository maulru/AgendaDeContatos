using Core.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Repository.Configurations
{
    public class RegiaoConfiguration : IEntityTypeConfiguration<Regiao>
    {
        public void Configure(EntityTypeBuilder<Regiao> builder)
        {
            builder.ToTable("Regiao");

            builder.HasKey(r => r.Id);
            builder.Property(r => r.Id).HasColumnType("INT").UseIdentityColumn();
            builder.Property(r => r.Nome).HasColumnName("Nome").HasColumnType("VARCHAR(100)").IsRequired();

            // Relacionamento com Estado
            builder.HasOne(r => r.Estado)
                   .WithMany(e => e.Regioes)
                   .HasForeignKey(r => r.EstadoId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
