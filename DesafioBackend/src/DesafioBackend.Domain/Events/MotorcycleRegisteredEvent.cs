using DesafioBackend.Domain.Common;

namespace DesafioBackend.Domain.Events;

public record MotorcycleRegisteredEvent(
    string MotorcycleId,
    int Year,
    string Model,
    string Plate
) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

