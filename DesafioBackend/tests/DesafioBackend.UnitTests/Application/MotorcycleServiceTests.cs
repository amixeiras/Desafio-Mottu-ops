using DesafioBackend.Application.Common.Exceptions;
using DesafioBackend.Application.Common.Interfaces;
using DesafioBackend.Application.DTOs;
using DesafioBackend.Application.Services;
using DesafioBackend.Domain.Entities;
using DesafioBackend.Domain.Events;
using DesafioBackend.Domain.Repositories;
using Moq;
using Xunit;

namespace DesafioBackend.UnitTests.Application;

public class MotorcycleServiceTests
{
    private readonly Mock<IMotorcycleRepository> _motorcycleRepoMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMessageBus> _messageBusMock;
    private readonly MotorcycleService _service;

    public MotorcycleServiceTests()
    {
        _motorcycleRepoMock = new Mock<IMotorcycleRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _messageBusMock = new Mock<IMessageBus>();

        _service = new MotorcycleService(
            _motorcycleRepoMock.Object,
            _unitOfWorkMock.Object,
            _messageBusMock.Object
        );
    }

    [Fact]
    public async Task CreateAsync_WhenPlateAlreadyExists_ThrowsValidationException()
    {
        // Arrange
        var request = new CreateMotorcycleRequest("moto1", 2024, "Sport", "ABC-1234");
        _motorcycleRepoMock.Setup(r => r.ExistsByPlateAsync(request.Placa, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => _service.CreateAsync(request));
        _motorcycleRepoMock.Verify(r => r.AddAsync(It.IsAny<Motorcycle>(), It.IsAny<CancellationToken>()), Times.Never);
        _messageBusMock.Verify(m => m.PublishAsync(It.IsAny<MotorcycleRegisteredEvent>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WhenValid_SavesAndPublishesEvent()
    {
        // Arrange
        var request = new CreateMotorcycleRequest("moto1", 2024, "Sport", "ABC-1234");
        _motorcycleRepoMock.Setup(r => r.ExistsByPlateAsync(request.Placa, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _motorcycleRepoMock.Setup(r => r.GetByIdAsync(request.Identificador, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Motorcycle?)null);

        // Act
        await _service.CreateAsync(request);

        // Assert
        _motorcycleRepoMock.Verify(r => r.AddAsync(It.IsAny<Motorcycle>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        _messageBusMock.Verify(m => m.PublishAsync(It.Is<MotorcycleRegisteredEvent>(e => e.Plate == "ABC-1234"), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WhenMotorcycleHasActiveRentals_ThrowsValidationException()
    {
        // Arrange
        var moto = new Motorcycle("moto1", 2024, "Sport", "ABC-1234");
        _motorcycleRepoMock.Setup(r => r.GetByIdAsync("moto1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(moto);
        _motorcycleRepoMock.Setup(r => r.HasActiveRentalsAsync("moto1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => _service.DeleteAsync("moto1"));
        _motorcycleRepoMock.Verify(r => r.Delete(It.IsAny<Motorcycle>()), Times.Never);
    }
}

