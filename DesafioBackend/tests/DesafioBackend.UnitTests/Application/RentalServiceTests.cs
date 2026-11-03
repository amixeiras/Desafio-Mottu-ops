using DesafioBackend.Application.Common.Exceptions;
using DesafioBackend.Application.DTOs;
using DesafioBackend.Application.Services;
using DesafioBackend.Domain.Entities;
using DesafioBackend.Domain.Enums;
using DesafioBackend.Domain.Repositories;
using Moq;
using Xunit;

namespace DesafioBackend.UnitTests.Application;

public class RentalServiceTests
{
    private readonly Mock<IRentalRepository> _rentalRepoMock;
    private readonly Mock<IDeliveryDriverRepository> _driverRepoMock;
    private readonly Mock<IMotorcycleRepository> _motorcycleRepoMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly RentalService _service;

    public RentalServiceTests()
    {
        _rentalRepoMock = new Mock<IRentalRepository>();
        _driverRepoMock = new Mock<IDeliveryDriverRepository>();
        _motorcycleRepoMock = new Mock<IMotorcycleRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _service = new RentalService(
            _rentalRepoMock.Object,
            _driverRepoMock.Object,
            _motorcycleRepoMock.Object,
            _unitOfWorkMock.Object
        );
    }

    [Fact]
    public async Task CreateAsync_WhenDriverHasOnlyCategoryB_ThrowsValidationException()
    {
        // Arrange
        var request = new CreateRentalRequest(
            "driver1",
            "moto1",
            DateTime.UtcNow.Date.AddDays(1),
            DateTime.UtcNow.Date.AddDays(8),
            DateTime.UtcNow.Date.AddDays(8),
            7
        );

        var driverWithBOnly = new DeliveryDriver("driver1", "Carlos", "11122233344", DateTime.UtcNow.AddYears(-25), "12345", DriverLicenseType.B);

        _driverRepoMock.Setup(d => d.GetByIdAsync("driver1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(driverWithBOnly);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<ValidationException>(() => _service.CreateAsync(request));
        Assert.Contains("categoria A", ex.Message);
    }

    [Fact]
    public async Task CreateAsync_WhenDriverAlreadyHasActiveRental_ThrowsValidationException()
    {
        // Arrange
        var request = new CreateRentalRequest(
            "driver1",
            "moto1",
            DateTime.UtcNow.Date.AddDays(1),
            DateTime.UtcNow.Date.AddDays(8),
            DateTime.UtcNow.Date.AddDays(8),
            7
        );

        var driver = new DeliveryDriver("driver1", "Carlos", "11122233344", DateTime.UtcNow.AddYears(-25), "12345", DriverLicenseType.A);
        _driverRepoMock.Setup(d => d.GetByIdAsync("driver1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(driver);

        var existingRental = new Rental("rent1", "driver1", "moto0", DateTime.UtcNow.AddDays(-2), DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(6), DateTime.UtcNow.AddDays(6), RentalPlanType.Days7);
        _rentalRepoMock.Setup(r => r.GetActiveRentalByDriverIdAsync("driver1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingRental);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<ValidationException>(() => _service.CreateAsync(request));
        Assert.Contains("locação ativa", ex.Message);
    }
}

