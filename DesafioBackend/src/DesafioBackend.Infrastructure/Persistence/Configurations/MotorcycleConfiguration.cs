using DesafioBackend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DesafioBackend.Infrastructure.Persistence.Configurations;

public class MotorcycleConfiguration : IEntityTypeConfiguration<Motorcycle>
{
    public void Configure(EntityTypeBuilder<Motorcycle> builder)
    {
        builder.ToTable("motos");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Id)
            .HasColumnName("identificador")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(m => m.Year)
            .HasColumnName("ano")
            .IsRequired();

        builder.Property(m => m.Model)
            .HasColumnName("modelo")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(m => m.Plate)
            .HasColumnName("placa")
            .HasMaxLength(20)
            .IsRequired();

        builder.HasIndex(m => m.Plate)
            .IsUnique();

        builder.Ignore(m => m.DomainEvents);
    }
}

