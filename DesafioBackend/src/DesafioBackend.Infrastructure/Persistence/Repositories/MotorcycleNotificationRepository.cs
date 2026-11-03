using DesafioBackend.Domain.Entities;
using DesafioBackend.Domain.Repositories;

namespace DesafioBackend.Infrastructure.Persistence.Repositories;

public class MotorcycleNotificationRepository : RepositoryBase<MotorcycleNotification, string>, IMotorcycleNotificationRepository
{
    public MotorcycleNotificationRepository(AppDbContext context) : base(context)
    {
    }
}

