using DesafioBackend.Domain.Entities;

namespace DesafioBackend.Domain.Repositories;

public interface IMotorcycleRepository : IRepository<Motorcycle, string>
{
    Task<Motorcycle?> GetByPlateAsync(string plate, CancellationToken cancellationToken = default);
    Task<bool> ExistsByPlateAsync(string plate, CancellationToken cancellationToken = default);
    Task<bool> HasActiveRentalsAsync(string motorcycleId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Motorcycle>> FilterByPlateAsync(string? plate, CancellationToken cancellationToken = default);
}
