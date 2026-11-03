using DesafioBackend.Domain.Common;
using DesafioBackend.Domain.Enums;
using DesafioBackend.Domain.Exceptions;

namespace DesafioBackend.Domain.Entities;

public class Delivery : BaseEntity<string>, IAggregateRoot
{
    public string OrderId { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string? DriverId { get; private set; }
    public DeliveryStatus Status { get; private set; }
    public DateTime? AcceptedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }

    protected Delivery() { }

    public Delivery(string id, string orderId, string description)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new DomainException("Identificador da entrega inválido.");
        if (string.IsNullOrWhiteSpace(orderId))
            throw new DomainException("Identificador do pedido é obrigatório.");
        if (string.IsNullOrWhiteSpace(description))
            throw new DomainException("Descrição do pedido é obrigatória.");

        Id = id;
        OrderId = orderId.Trim();
        Description = description.Trim();
        Status = DeliveryStatus.Available;
    }

    public void Accept(string driverId, DateTime acceptedAt)
    {
        if (Status != DeliveryStatus.Available)
            throw new DomainException("A entrega não está disponível.");
        if (string.IsNullOrWhiteSpace(driverId))
            throw new DomainException("Entregador inválido.");

        DriverId = driverId;
        AcceptedAt = acceptedAt;
        Status = DeliveryStatus.Accepted;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Complete(DateTime completedAt)
    {
        if (Status != DeliveryStatus.Accepted)
            throw new DomainException("Somente entregas aceitas podem ser concluídas.");

        CompletedAt = completedAt;
        Status = DeliveryStatus.Completed;
        UpdatedAt = DateTime.UtcNow;
    }
}
