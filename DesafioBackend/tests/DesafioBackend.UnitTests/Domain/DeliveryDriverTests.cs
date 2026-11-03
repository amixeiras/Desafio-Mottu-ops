using DesafioBackend.Domain.Entities;
using DesafioBackend.Domain.Enums;
using Xunit;

namespace DesafioBackend.UnitTests.Domain;

public class DeliveryDriverTests
{
    [Theory]
    [InlineData(DriverLicenseType.A, true)]
    [InlineData(DriverLicenseType.AB, true)]
    [InlineData(DriverLicenseType.B, false)]
    public void CanRentMotorcycle_ShouldOnlyAllowCategoryAOrAB(DriverLicenseType cnhType, bool expectedCanRent)
    {
        // Arrange
        var driver = new DeliveryDriver(
            "d1",
            "João Silva",
            "12345678000199",
            new DateTime(1990, 5, 10),
            "12345678900",
            cnhType
        );

        // Act
        var canRent = driver.CanRentMotorcycle();

        // Assert
        Assert.Equal(expectedCanRent, canRent);
    }

    [Fact]
    public void UpdateCnhImage_ShouldUpdateUrlAndTimestamp()
    {
        // Arrange
        var driver = new DeliveryDriver(
            "d1",
            "João Silva",
            "12345678000199",
            new DateTime(1990, 5, 10),
            "12345678900",
            DriverLicenseType.A
        );

        // Act
        driver.UpdateCnhImage("uploads/cnh/test.png");

        // Assert
        Assert.Equal("uploads/cnh/test.png", driver.CnhImageUrl);
        Assert.NotNull(driver.UpdatedAt);
    }
}

