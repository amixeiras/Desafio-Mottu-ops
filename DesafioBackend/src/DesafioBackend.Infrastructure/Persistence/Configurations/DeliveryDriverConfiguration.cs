using DesafioBackend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DesafioBackend.Infrastructure.Persistence.Configurations;

public class DeliveryDriverConfiguration : IEntityTypeConfiguration<DeliveryDriver>
{
    public void Configure(EntityTypeBuilder<DeliveryDriver> builder)
    {
        builder.ToTable("entregadores");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.Id)
            .HasColumnName("identificador")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(d => d.Name)
            .HasColumnName("nome")
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(d => d.Cnpj)
            .HasColumnName("cnpj")
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(d => d.BirthDate)
            .HasColumnName("data_nascimento")
            .IsRequired();

        builder.Property(d => d.CnhNumber)
            .HasColumnName("numero_cnh")
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(d => d.CnhType)
            .HasColumnName("tipo_cnh")
            .HasConversion<string>()
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(d => d.CnhImageUrl)
            .HasColumnName("imagem_cnh")
            .HasMaxLength(500)
            .IsRequired(false);

        builder.HasIndex(d => d.Cnpj)
            .IsUnique();

        builder.HasIndex(d => d.CnhNumber)
            .IsUnique();

        builder.Ignore(d => d.DomainEvents);
    }
}

