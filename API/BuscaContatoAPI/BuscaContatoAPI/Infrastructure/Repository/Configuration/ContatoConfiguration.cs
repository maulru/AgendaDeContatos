using Core.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Repository.Configurations
{
    public class ContatoConfiguration : IEntityTypeConfiguration<Contato>
    {
        public void Configure(EntityTypeBuilder<Contato> builder)
        {
            builder.ToTable("Contato");

            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id).HasColumnType("INT").UseIdentityColumn();
            builder.Property(p => p.Nome).HasColumnName("Nome").HasColumnType("VARCHAR(100)").IsRequired();
            builder.Property(p => p.Email).HasColumnName("Email").HasColumnType("VARCHAR(100)").IsRequired();

            // Relacionamento com Telefone
            builder.HasMany(p => p.Telefones)
                   .WithOne(t => t.Contato)
                   .HasForeignKey(t => t.ContatoId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
