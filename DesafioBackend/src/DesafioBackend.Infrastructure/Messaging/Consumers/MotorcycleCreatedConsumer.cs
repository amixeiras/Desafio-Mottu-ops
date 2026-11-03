using DesafioBackend.Domain.Entities;
using DesafioBackend.Domain.Events;
using DesafioBackend.Domain.Repositories;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace DesafioBackend.Infrastructure.Messaging.Consumers;

public class MotorcycleCreatedConsumer : IConsumer<MotorcycleRegisteredEvent>
{
    private readonly IMotorcycleNotificationRepository _notificationRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<MotorcycleCreatedConsumer> _logger;

    public MotorcycleCreatedConsumer(
        IMotorcycleNotificationRepository notificationRepository,
        IUnitOfWork unitOfWork,
        ILogger<MotorcycleCreatedConsumer> logger)
    {
        _notificationRepository = notificationRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<MotorcycleRegisteredEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("Recebido evento de moto cadastrada: Id={Id}, Ano={Year}, Placa={Plate}", 
            message.MotorcycleId, message.Year, message.Plate);

        // Notificar e armazenar quando o ano for 2024
        if (message.Year == 2024)
        {
            _logger.LogInformation("Moto do ano 2024 detectada! Registrando notificação para a moto {Id}", message.MotorcycleId);

            var notification = new MotorcycleNotification(
                id: Guid.NewGuid().ToString(),
                motorcycleId: message.MotorcycleId,
                year: message.Year,
                model: message.Model,
                plate: message.Plate,
                message: $"Moto ano 2024 cadastrada com sucesso com a placa {message.Plate}."
            );

            await _notificationRepository.AddAsync(notification, context.CancellationToken);
            await _unitOfWork.CommitAsync(context.CancellationToken);

            _logger.LogInformation("Notificação da moto 2024 gravada com sucesso no banco de dados.");
        }
    }
}

