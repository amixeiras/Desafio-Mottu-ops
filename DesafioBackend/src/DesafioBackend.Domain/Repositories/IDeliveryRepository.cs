using DesafioBackend.Domain.Entities;
using DesafioBackend.Domain.Enums;

namespace DesafioBackend.Domain.Repositories;

public interface IDeliveryRepository : IRepository<Delivery, string>
{
    Task<IReadOnlyList<Delivery>> GetByStatusAsync(DeliveryStatus status, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Delivery>> GetByDriverIdAsync(string driverId, CancellationToken cancellationToken = default);
}
