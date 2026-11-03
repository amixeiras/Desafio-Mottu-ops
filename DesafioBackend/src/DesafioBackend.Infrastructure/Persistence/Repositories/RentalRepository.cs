using DesafioBackend.Domain.Entities;
using DesafioBackend.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DesafioBackend.Infrastructure.Persistence.Repositories;

public class RentalRepository : RepositoryBase<Rental, string>, IRentalRepository
{
    public RentalRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<Rental?> GetActiveRentalByDriverIdAsync(string driverId, CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(
            r => r.DriverId == driverId && r.ReturnDate == null, 
            cancellationToken);
    }

    public async Task<Rental?> GetActiveRentalByMotorcycleIdAsync(string motorcycleId, CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(
            r => r.MotorcycleId == motorcycleId && r.ReturnDate == null, 
            cancellationToken);
    }
}

