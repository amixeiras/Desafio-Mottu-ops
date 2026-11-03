using DesafioBackend.Domain.Entities;
using DesafioBackend.Domain.Events;
using DesafioBackend.Domain.Exceptions;
using Xunit;

namespace DesafioBackend.UnitTests.Domain;

public class MotorcycleTests
{
    [Fact]
    public void Create_WithValidData_ShouldCreateInstanceAndAddDomainEvent()
    {
        // Act
        var moto = new Motorcycle("m1", 2024, "Mottu Sport", "ABC-1234");

        // Assert
        Assert.Equal("m1", moto.Id);
        Assert.Equal(2024, moto.Year);
        Assert.Equal("Mottu Sport", moto.Model);
        Assert.Equal("ABC-1234", moto.Plate);
        Assert.Single(moto.DomainEvents);
        Assert.IsType<MotorcycleRegisteredEvent>(moto.DomainEvents.First());
    }

    [Theory]
    [InlineData("", 2024, "Model", "ABC1234")]
    [InlineData("m1", 1899, "Model", "ABC1234")]
    [InlineData("m1", 2024, "", "ABC1234")]
    [InlineData("m1", 2024, "Model", "")]
    public void Create_WithInvalidData_ShouldThrowDomainException(string id, int year, string model, string plate)
    {
        // Act & Assert
        Assert.Throws<DomainException>(() => new Motorcycle(id, year, model, plate));
    }

    [Fact]
    public void UpdatePlate_WithValidPlate_ShouldUpdateAndSetUpdatedAt()
    {
        // Arrange
        var moto = new Motorcycle("m1", 2024, "Mottu Sport", "ABC-1234");

        // Act
        moto.UpdatePlate("XYZ-9999");

        // Assert
        Assert.Equal("XYZ-9999", moto.Plate);
        Assert.NotNull(moto.UpdatedAt);
    }
}

