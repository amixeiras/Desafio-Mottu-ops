using DesafioBackend.Domain.Entities;

namespace DesafioBackend.Domain.Repositories;

public interface IRentalRepository : IRepository<Rental, string>
{
    Task<Rental?> GetActiveRentalByDriverIdAsync(string driverId, CancellationToken cancellationToken = default);
    Task<Rental?> GetActiveRentalByMotorcycleIdAsync(string motorcycleId, CancellationToken cancellationToken = default);
}

