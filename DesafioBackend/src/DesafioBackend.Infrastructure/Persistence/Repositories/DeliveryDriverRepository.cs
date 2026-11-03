using DesafioBackend.Domain.Entities;
using DesafioBackend.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DesafioBackend.Infrastructure.Persistence.Repositories;

public class DeliveryDriverRepository : RepositoryBase<DeliveryDriver, string>, IDeliveryDriverRepository
{
    public DeliveryDriverRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<bool> ExistsByCnpjAsync(string cnpj, CancellationToken cancellationToken = default)
    {
        var normalizedCnpj = cnpj.Trim();
        return await DbSet.AnyAsync(d => d.Cnpj == normalizedCnpj, cancellationToken);
    }

    public async Task<bool> ExistsByCnhNumberAsync(string cnhNumber, CancellationToken cancellationToken = default)
    {
        var normalizedCnh = cnhNumber.Trim();
        return await DbSet.AnyAsync(d => d.CnhNumber == normalizedCnh, cancellationToken);
    }
}

