using DesafioBackend.Domain.Entities;
using DesafioBackend.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DesafioBackend.Infrastructure.Persistence.Repositories;

public class MotorcycleRepository : RepositoryBase<Motorcycle, string>, IMotorcycleRepository
{
    public MotorcycleRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<Motorcycle?> GetByPlateAsync(string plate, CancellationToken cancellationToken = default)
    {
        var normalizedPlate = plate.Trim().ToUpperInvariant();
        return await DbSet.FirstOrDefaultAsync(m => m.Plate == normalizedPlate, cancellationToken);
    }

    public async Task<bool> ExistsByPlateAsync(string plate, CancellationToken cancellationToken = default)
    {
        var normalizedPlate = plate.Trim().ToUpperInvariant();
        return await DbSet.AnyAsync(m => m.Plate == normalizedPlate, cancellationToken);
    }

    public async Task<bool> HasActiveRentalsAsync(string motorcycleId, CancellationToken cancellationToken = default)
    {
        return await Context.Rentals.AnyAsync(r => r.MotorcycleId == motorcycleId, cancellationToken);
    }

    public async Task<IReadOnlyList<Motorcycle>> FilterByPlateAsync(string? plate, CancellationToken cancellationToken = default)
    {
        IQueryable<Motorcycle> query = DbSet;

        if (!string.IsNullOrWhiteSpace(plate))
        {
            var normalizedPlate = plate.Trim().ToUpperInvariant();
            query = query.Where(m => m.Plate.Contains(normalizedPlate));
        }

        return await query.ToListAsync(cancellationToken);
    }
}

