using DesafioBackend.Application.DTOs;

namespace DesafioBackend.Application.Services;

public interface IDeliveryService
{
    Task<string> CreateAsync(CreateDeliveryRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DeliveryResponse>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DeliveryResponse>> GetAvailableAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DeliveryResponse>> GetByStatusAsync(string status, CancellationToken cancellationToken = default);
    Task<DeliveryResponse> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DeliveryResponse>> GetByDriverAsync(string driverId, CancellationToken cancellationToken = default);
    Task AcceptAsync(string id, AcceptDeliveryRequest request, CancellationToken cancellationToken = default);
    Task CompleteAsync(string id, AcceptDeliveryRequest request, CancellationToken cancellationToken = default);
}
