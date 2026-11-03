using DesafioBackend.Domain.Common;

namespace DesafioBackend.Domain.Entities;

public class MotorcycleNotification : BaseEntity<string>, IAggregateRoot
{
    public string MotorcycleId { get; private set; } = string.Empty;
    public int Year { get; private set; }
    public string Model { get; private set; } = string.Empty;
    public string Plate { get; private set; } = string.Empty;
    public string Message { get; private set; } = string.Empty;

    protected MotorcycleNotification() { }

    public MotorcycleNotification(string id, string motorcycleId, int year, string model, string plate, string message)
    {
        Id = id;
        MotorcycleId = motorcycleId;
        Year = year;
        Model = model;
        Plate = plate;
        Message = message;
        CreatedAt = DateTime.UtcNow;
    }
}

