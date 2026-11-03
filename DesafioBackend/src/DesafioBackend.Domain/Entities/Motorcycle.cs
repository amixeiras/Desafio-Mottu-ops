using DesafioBackend.Domain.Common;
using DesafioBackend.Domain.Events;
using DesafioBackend.Domain.Exceptions;

namespace DesafioBackend.Domain.Entities;

public class Motorcycle : BaseEntity<string>, IAggregateRoot
{
    public int Year { get; private set; }
    public string Model { get; private set; } = string.Empty;
    public string Plate { get; private set; } = string.Empty;

    // For EF Core
    protected Motorcycle() { }

    public Motorcycle(string id, int year, string model, string plate)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new DomainException("Identificador inválido.");
        if (year <= 1900 || year > DateTime.UtcNow.Year + 1)
            throw new DomainException("Ano inválido.");
        if (string.IsNullOrWhiteSpace(model))
            throw new DomainException("Modelo é obrigatório.");
        if (string.IsNullOrWhiteSpace(plate))
            throw new DomainException("Placa é obrigatória.");

        Id = id;
        Year = year;
        Model = model;
        Plate = plate.Trim().ToUpperInvariant();

        AddDomainEvent(new MotorcycleRegisteredEvent(Id, Year, Model, Plate));
    }

    public void UpdatePlate(string newPlate)
    {
        if (string.IsNullOrWhiteSpace(newPlate))
            throw new DomainException("A placa não pode ser vazia.");

        Plate = newPlate.Trim().ToUpperInvariant();
        UpdatedAt = DateTime.UtcNow;
    }
}

