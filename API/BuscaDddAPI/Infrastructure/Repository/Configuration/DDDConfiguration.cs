using Core.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Repository.Configurations
{
    internal class DDDConfiguration : IEntityTypeConfiguration<DDD>
    {
        public void Configure(EntityTypeBuilder<DDD> builder)
        {
            builder.ToTable("DDD");

            builder.HasKey(d => d.Id);
            builder.Property(d => d.Id).HasColumnType("INT").UseIdentityColumn();
            builder.Property(d => d.Codigo).HasColumnName("Codigo").HasColumnType("VARCHAR(10)").IsRequired();
        }

    }
}
