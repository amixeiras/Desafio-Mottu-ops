using DesafioBackend.Domain.Entities;
using DesafioBackend.Domain.Enums;
using DesafioBackend.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DesafioBackend.Infrastructure.Persistence.Repositories;

public sealed class DeliveryRepository(AppDbContext context)
    : RepositoryBase<Delivery, string>(context), IDeliveryRepository
{
    public async Task<IReadOnlyList<Delivery>> GetByStatusAsync(
        DeliveryStatus status,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(delivery => delivery.Status == status)
            .OrderBy(delivery => delivery.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Delivery>> GetByDriverIdAsync(
        string driverId,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(delivery => delivery.DriverId == driverId)
            .OrderByDescending(delivery => delivery.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}
