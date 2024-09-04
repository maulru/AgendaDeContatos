using Core.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Repository.Configurations
{
    public class TelefoneConfiguration : IEntityTypeConfiguration<Telefone>
    {
        public void Configure(EntityTypeBuilder<Telefone> builder)
        {
            builder.ToTable("Telefone");

            builder.HasKey(t => t.Id);
            builder.Property(t => t.Id).HasColumnType("INT").UseIdentityColumn();
            builder.Property(t => t.NumeroTelefone).HasColumnName("NumeroTelefone").HasColumnType("VARCHAR(20)").IsRequired();

            // Relacionamento com Contato
            builder.HasOne(t => t.Contato)
                   .WithMany(c => c.Telefones)
                   .HasForeignKey(t => t.ContatoId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Relacionamento com DDD
            builder.HasOne(t => t.DDD)
                   .WithMany(d => d.Telefones)
                   .HasForeignKey(t => t.DDDId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
