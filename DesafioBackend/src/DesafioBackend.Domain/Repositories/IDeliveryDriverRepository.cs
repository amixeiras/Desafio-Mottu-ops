using DesafioBackend.Domain.Entities;

namespace DesafioBackend.Domain.Repositories;

public interface IDeliveryDriverRepository : IRepository<DeliveryDriver, string>
{
    Task<bool> ExistsByCnpjAsync(string cnpj, CancellationToken cancellationToken = default);
    Task<bool> ExistsByCnhNumberAsync(string cnhNumber, CancellationToken cancellationToken = default);
}

