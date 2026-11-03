using DesafioBackend.Application.Common.Exceptions;
using DesafioBackend.Application.DTOs;
using DesafioBackend.Domain.Entities;
using DesafioBackend.Domain.Enums;
using DesafioBackend.Domain.Repositories;

namespace DesafioBackend.Application.Services;

public class RentalService : IRentalService
{
    private readonly IRentalRepository _rentalRepository;
    private readonly IDeliveryDriverRepository _driverRepository;
    private readonly IMotorcycleRepository _motorcycleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RentalService(
        IRentalRepository rentalRepository,
        IDeliveryDriverRepository driverRepository,
        IMotorcycleRepository motorcycleRepository,
        IUnitOfWork unitOfWork)
    {
        _rentalRepository = rentalRepository;
        _driverRepository = driverRepository;
        _motorcycleRepository = motorcycleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<string> CreateAsync(CreateRentalRequest request, CancellationToken cancellationToken = default)
    {
        if (request is null)
            throw new ValidationException("Dados da locação inválidos.");

        // Validar plano
        if (!Enum.IsDefined(typeof(RentalPlanType), request.Plano))
        {
            throw new ValidationException("Plano selecionado é inválido. Opções válidas: 7, 15, 30, 45 ou 50 dias.");
        }
        var plan = (RentalPlanType)request.Plano;

        // Validar entregador
        var driver = await _driverRepository.GetByIdAsync(request.EntregadorId, cancellationToken);
        if (driver is null)
        {
            throw new ValidationException("Entregador não encontrado.");
        }

        if (!driver.CanRentMotorcycle())
        {
            throw new ValidationException("Somente entregadores com CNH categoria A ou A+B podem realizar locações.");
        }

        var activeRentalForDriver = await _rentalRepository.GetActiveRentalByDriverIdAsync(request.EntregadorId, cancellationToken);
        if (activeRentalForDriver != null)
        {
            throw new ValidationException("O entregador já possui uma locação ativa.");
        }

        // Validar moto
        var motorcycle = await _motorcycleRepository.GetByIdAsync(request.MotoId, cancellationToken);
        if (motorcycle is null)
        {
            throw new ValidationException("Moto não encontrada.");
        }

        var activeRentalForMoto = await _rentalRepository.GetActiveRentalByMotorcycleIdAsync(request.MotoId, cancellationToken);
        if (activeRentalForMoto != null)
        {
            throw new ValidationException("A moto já está locada por outro entregador.");
        }

        var now = DateTime.UtcNow;
        var rentalId = Guid.NewGuid().ToString();

        var rental = new Rental(
            rentalId,
            request.EntregadorId,
            request.MotoId,
            now,
            request.DataInicio,
            request.DataTermino,
            request.DataPrevisaoTermino,
            plan
        );

        await _rentalRepository.AddAsync(rental, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return rental.Id;
    }

    public async Task<RentalResponse> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ValidationException("Identificador inválido.");

        var rental = await _rentalRepository.GetByIdAsync(id, cancellationToken);
        if (rental is null)
        {
            throw new NotFoundException("Dados não encontrados");
        }

        return new RentalResponse(
            rental.Id,
            rental.DailyRate,
            rental.DriverId,
            rental.MotorcycleId,
            rental.StartDate,
            rental.EndDate,
            rental.ExpectedEndDate,
            rental.ReturnDate,
            rental.TotalCost
        );
    }

    public async Task<IReadOnlyList<RentalResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var rentals = await _rentalRepository.GetAllAsync(cancellationToken);
        return rentals.Select(rental => new RentalResponse(
            rental.Id,
            rental.DailyRate,
            rental.DriverId,
            rental.MotorcycleId,
            rental.StartDate,
            rental.EndDate,
            rental.ExpectedEndDate,
            rental.ReturnDate,
            rental.TotalCost)).ToList();
    }

    public async Task<decimal> InformReturnDateAsync(string id, ReturnRentalRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ValidationException("Identificador inválido.");

        if (request is null || request.DataDevolucao == default)
            throw new ValidationException("Data de devolução é obrigatória.");

        var rental = await _rentalRepository.GetByIdAsync(id, cancellationToken);
        if (rental is null)
        {
            throw new NotFoundException("Dados não encontrados");
        }

        var total = rental.CompleteReturn(request.DataDevolucao);
        _rentalRepository.Update(rental);
        await _unitOfWork.CommitAsync(cancellationToken);

        return total;
    }
}

