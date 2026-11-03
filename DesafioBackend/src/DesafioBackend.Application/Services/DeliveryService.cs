using DesafioBackend.Application.Common.Exceptions;
using DesafioBackend.Application.DTOs;
using DesafioBackend.Domain.Entities;
using DesafioBackend.Domain.Enums;
using DesafioBackend.Domain.Repositories;

namespace DesafioBackend.Application.Services;

public sealed class DeliveryService(
    IDeliveryRepository deliveryRepository,
    IDeliveryDriverRepository driverRepository,
    IRentalRepository rentalRepository,
    IUnitOfWork unitOfWork) : IDeliveryService
{
    public async Task<string> CreateAsync(
        CreateDeliveryRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request is null || string.IsNullOrWhiteSpace(request.PedidoId) || string.IsNullOrWhiteSpace(request.Descricao))
            throw new ValidationException("Pedido e descrição são obrigatórios.");

        var delivery = new Delivery(Guid.NewGuid().ToString(), request.PedidoId, request.Descricao);
        await deliveryRepository.AddAsync(delivery, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);
        return delivery.Id;
    }

    public async Task<IReadOnlyList<DeliveryResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var deliveries = await deliveryRepository.GetAllAsync(cancellationToken);
        return deliveries.Select(ToResponse).ToList();
    }

    public async Task<IReadOnlyList<DeliveryResponse>> GetAvailableAsync(CancellationToken cancellationToken = default)
    {
        var deliveries = await deliveryRepository.GetByStatusAsync(DeliveryStatus.Available, cancellationToken);
        return deliveries.Select(ToResponse).ToList();
    }

    public async Task<IReadOnlyList<DeliveryResponse>> GetByStatusAsync(string status, CancellationToken cancellationToken = default)
    {
        if (Enum.TryParse<DeliveryStatus>(status, true, out var parsedStatus))
        {
            var deliveries = await deliveryRepository.GetByStatusAsync(parsedStatus, cancellationToken);
            return deliveries.Select(ToResponse).ToList();
        }
        return [];
    }

    public async Task<DeliveryResponse> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return ToResponse(await GetDeliveryAsync(id, cancellationToken));
    }

    public async Task<IReadOnlyList<DeliveryResponse>> GetByDriverAsync(
        string driverId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(driverId))
            throw new ValidationException("Entregador inválido.");

        var deliveries = await deliveryRepository.GetByDriverIdAsync(driverId, cancellationToken);
        return deliveries.Select(ToResponse).ToList();
    }

    public async Task AcceptAsync(
        string id,
        AcceptDeliveryRequest request,
        CancellationToken cancellationToken = default)
    {
        var delivery = await GetDeliveryAsync(id, cancellationToken);
        var driver = await GetDriverAsync(request?.EntregadorId, cancellationToken);
        var activeRental = await rentalRepository.GetActiveRentalByDriverIdAsync(driver.Id, cancellationToken);

        if (activeRental is null)
            throw new ValidationException("O entregador precisa possuir uma locação ativa para aceitar entregas.");

        delivery.Accept(driver.Id, DateTime.UtcNow);
        deliveryRepository.Update(delivery);
        await unitOfWork.CommitAsync(cancellationToken);
    }

    public async Task CompleteAsync(
        string id,
        AcceptDeliveryRequest request,
        CancellationToken cancellationToken = default)
    {
        var delivery = await GetDeliveryAsync(id, cancellationToken);
        var driverId = request?.EntregadorId;

        if (string.IsNullOrWhiteSpace(driverId) || !string.Equals(delivery.DriverId, driverId, StringComparison.Ordinal))
            throw new ValidationException("Somente o entregador que aceitou a entrega pode concluí-la.");

        delivery.Complete(DateTime.UtcNow);
        deliveryRepository.Update(delivery);
        await unitOfWork.CommitAsync(cancellationToken);
    }

    private async Task<Delivery> GetDeliveryAsync(string id, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ValidationException("Identificador da entrega inválido.");

        return await deliveryRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("Entrega não encontrada.");
    }

    private async Task<DeliveryDriver> GetDriverAsync(string? id, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ValidationException("Entregador é obrigatório.");

        return await driverRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("Entregador não encontrado.");
    }

    private static DeliveryResponse ToResponse(Delivery delivery) => new(
        delivery.Id,
        delivery.OrderId,
        delivery.Description,
        delivery.DriverId,
        delivery.Status.ToString(),
        delivery.AcceptedAt,
        delivery.CompletedAt);
}
