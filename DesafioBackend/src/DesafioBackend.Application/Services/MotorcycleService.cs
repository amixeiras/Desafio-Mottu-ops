using DesafioBackend.Application.Common.Exceptions;
using DesafioBackend.Application.Common.Interfaces;
using DesafioBackend.Application.DTOs;
using DesafioBackend.Domain.Entities;
using DesafioBackend.Domain.Events;
using DesafioBackend.Domain.Repositories;

namespace DesafioBackend.Application.Services;

public class MotorcycleService : IMotorcycleService
{
    private readonly IMotorcycleRepository _motorcycleRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMessageBus _messageBus;

    public MotorcycleService(
        IMotorcycleRepository motorcycleRepository,
        IUnitOfWork unitOfWork,
        IMessageBus messageBus)
    {
        _motorcycleRepository = motorcycleRepository;
        _unitOfWork = unitOfWork;
        _messageBus = messageBus;
    }

    public async Task CreateAsync(CreateMotorcycleRequest request, CancellationToken cancellationToken = default)
    {
        if (request is null)
            throw new ValidationException("Dados da requisição inválidos.");

        if (string.IsNullOrWhiteSpace(request.Identificador) ||
            string.IsNullOrWhiteSpace(request.Modelo) ||
            string.IsNullOrWhiteSpace(request.Placa))
        {
            throw new ValidationException("Todos os campos da moto são obrigatórios.");
        }

        var plateExists = await _motorcycleRepository.ExistsByPlateAsync(request.Placa, cancellationToken);
        if (plateExists)
        {
            throw new ValidationException("A placa informada já está cadastrada.");
        }

        var idExists = await _motorcycleRepository.GetByIdAsync(request.Identificador, cancellationToken);
        if (idExists != null)
        {
            throw new ValidationException("O identificador informado já está cadastrado.");
        }

        var motorcycle = new Motorcycle(request.Identificador, request.Ano, request.Modelo, request.Placa);

        await _motorcycleRepository.AddAsync(motorcycle, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        // Notificação de mensageria
        await _messageBus.PublishAsync(
            new MotorcycleRegisteredEvent(motorcycle.Id, motorcycle.Year, motorcycle.Model, motorcycle.Plate), 
            cancellationToken);
    }

    public async Task<IReadOnlyList<MotorcycleResponse>> GetAllAsync(string? plate, CancellationToken cancellationToken = default)
    {
        var list = await _motorcycleRepository.FilterByPlateAsync(plate, cancellationToken);
        return list.Select(m => new MotorcycleResponse(m.Id, m.Year, m.Model, m.Plate)).ToList();
    }

    public async Task<MotorcycleResponse> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ValidationException("Identificador inválido.");

        var motorcycle = await _motorcycleRepository.GetByIdAsync(id, cancellationToken);
        if (motorcycle is null)
        {
            throw new NotFoundException("Moto não encontrada");
        }

        return new MotorcycleResponse(motorcycle.Id, motorcycle.Year, motorcycle.Model, motorcycle.Plate);
    }

    public async Task UpdatePlateAsync(string id, UpdateMotorcyclePlateRequest request, CancellationToken cancellationToken = default)
    {
        if (request is null || string.IsNullOrWhiteSpace(request.Placa))
            throw new ValidationException("A nova placa é obrigatória.");

        var motorcycle = await _motorcycleRepository.GetByIdAsync(id, cancellationToken);
        if (motorcycle is null)
        {
            throw new NotFoundException("Moto não encontrada");
        }

        var existingWithPlate = await _motorcycleRepository.GetByPlateAsync(request.Placa, cancellationToken);
        if (existingWithPlate != null && existingWithPlate.Id != id)
        {
            throw new ValidationException("A placa informada já está cadastrada para outra moto.");
        }

        motorcycle.UpdatePlate(request.Placa);
        _motorcycleRepository.Update(motorcycle);
        await _unitOfWork.CommitAsync(cancellationToken);
    }

    public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ValidationException("Identificador inválido.");

        var motorcycle = await _motorcycleRepository.GetByIdAsync(id, cancellationToken);
        if (motorcycle is null)
        {
            throw new NotFoundException("Moto não encontrada");
        }

        var hasRentals = await _motorcycleRepository.HasActiveRentalsAsync(id, cancellationToken);
        if (hasRentals)
        {
            throw new ValidationException("Moto possui locações registradas e não pode ser removida.");
        }

        _motorcycleRepository.Delete(motorcycle);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}

