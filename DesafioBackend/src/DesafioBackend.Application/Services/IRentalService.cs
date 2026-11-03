using DesafioBackend.Application.DTOs;

namespace DesafioBackend.Application.Services;

public interface IRentalService
{
    Task<string> CreateAsync(CreateRentalRequest request, CancellationToken cancellationToken = default);
    Task<RentalResponse> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RentalResponse>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<decimal> InformReturnDateAsync(string id, ReturnRentalRequest request, CancellationToken cancellationToken = default);
}

