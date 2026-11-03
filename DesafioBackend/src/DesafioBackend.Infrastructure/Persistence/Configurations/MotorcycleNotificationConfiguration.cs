using DesafioBackend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DesafioBackend.Infrastructure.Persistence.Configurations;

public class MotorcycleNotificationConfiguration : IEntityTypeConfiguration<MotorcycleNotification>
{
    public void Configure(EntityTypeBuilder<MotorcycleNotification> builder)
    {
        builder.ToTable("notificacoes_motos");

        builder.HasKey(n => n.Id);

        builder.Property(n => n.Id)
            .HasColumnName("identificador")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(n => n.MotorcycleId)
            .HasColumnName("moto_id")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(n => n.Year)
            .HasColumnName("ano")
            .IsRequired();

        builder.Property(n => n.Model)
            .HasColumnName("modelo")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(n => n.Plate)
            .HasColumnName("placa")
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(n => n.Message)
            .HasColumnName("mensagem")
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(n => n.CreatedAt)
            .HasColumnName("data_notificacao")
            .IsRequired();

        builder.Ignore(n => n.DomainEvents);
    }
}

