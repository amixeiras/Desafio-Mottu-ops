using DesafioBackend.Application.DTOs;

namespace DesafioBackend.Application.Services;

public interface IMotorcycleService
{
    Task CreateAsync(CreateMotorcycleRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MotorcycleResponse>> GetAllAsync(string? plate, CancellationToken cancellationToken = default);
    Task<MotorcycleResponse> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task UpdatePlateAsync(string id, UpdateMotorcyclePlateRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(string id, CancellationToken cancellationToken = default);
}

