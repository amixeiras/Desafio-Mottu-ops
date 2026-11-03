using DesafioBackend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DesafioBackend.Infrastructure.Persistence.Configurations;

public class RentalConfiguration : IEntityTypeConfiguration<Rental>
{
    public void Configure(EntityTypeBuilder<Rental> builder)
    {
        builder.ToTable("locacoes");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id)
            .HasColumnName("identificador")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(r => r.DriverId)
            .HasColumnName("entregador_id")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(r => r.MotorcycleId)
            .HasColumnName("moto_id")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(r => r.StartDate)
            .HasColumnName("data_inicio")
            .IsRequired();

        builder.Property(r => r.EndDate)
            .HasColumnName("data_termino")
            .IsRequired();

        builder.Property(r => r.ExpectedEndDate)
            .HasColumnName("data_previsao_termino")
            .IsRequired();

        builder.Property(r => r.ReturnDate)
            .HasColumnName("data_devolucao")
            .IsRequired(false);

        builder.Property(r => r.Plan)
            .HasColumnName("plano")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(r => r.DailyRate)
            .HasColumnName("valor_diaria")
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(r => r.TotalCost)
            .HasColumnName("valor_total")
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Ignore(r => r.DomainEvents);
    }
}

