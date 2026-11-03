using DesafioBackend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DesafioBackend.Infrastructure.Persistence.Configurations;

public sealed class DeliveryConfiguration : IEntityTypeConfiguration<Delivery>
{
    public void Configure(EntityTypeBuilder<Delivery> builder)
    {
        builder.ToTable("entregas");
        builder.HasKey(delivery => delivery.Id);

        builder.Property(delivery => delivery.Id)
            .HasColumnName("identificador")
            .HasMaxLength(100)
            .IsRequired();
        builder.Property(delivery => delivery.OrderId)
            .HasColumnName("pedido_id")
            .HasMaxLength(100)
            .IsRequired();
        builder.Property(delivery => delivery.Description)
            .HasColumnName("descricao")
            .HasMaxLength(500)
            .IsRequired();
        builder.Property(delivery => delivery.DriverId)
            .HasColumnName("entregador_id")
            .HasMaxLength(100);
        builder.Property(delivery => delivery.Status)
            .HasColumnName("status")
            .HasConversion<int>()
            .IsRequired();
        builder.Property(delivery => delivery.AcceptedAt).HasColumnName("aceita_em");
        builder.Property(delivery => delivery.CompletedAt).HasColumnName("concluida_em");
        builder.Property(delivery => delivery.CreatedAt).HasColumnName("criada_em").IsRequired();
        builder.Property(delivery => delivery.UpdatedAt).HasColumnName("atualizada_em");
        builder.Ignore(delivery => delivery.DomainEvents);

        builder.HasIndex(delivery => delivery.OrderId).IsUnique();
    }
}
