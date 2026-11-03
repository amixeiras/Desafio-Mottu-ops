using DesafioBackend.Application.DTOs;

namespace DesafioBackend.Application.Services;

public interface IDeliveryDriverService
{
    Task CreateAsync(CreateDeliveryDriverRequest request, CancellationToken cancellationToken = default);
    Task UpdateCnhImageAsync(string id, UpdateCnhImageRequest request, CancellationToken cancellationToken = default);
    Task<DeliveryDriverResponse> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DeliveryDriverResponse>> GetAllAsync(CancellationToken cancellationToken = default);
}

