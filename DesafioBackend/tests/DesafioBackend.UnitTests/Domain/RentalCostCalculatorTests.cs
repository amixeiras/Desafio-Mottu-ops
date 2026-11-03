using DesafioBackend.Domain.Entities;
using DesafioBackend.Domain.Enums;
using Xunit;

namespace DesafioBackend.UnitTests.Domain;

public class RentalCostCalculatorTests
{
    [Fact]
    public void CalculateFinalCost_WhenReturnedOnExpectedDate_ReturnsStandardTotal()
    {
        // Arrange: 7-day plan at R$ 30/day = R$ 210
        var createdAt = new DateTime(2026, 10, 1);
        var startDate = createdAt.Date.AddDays(1);
        var expectedEnd = startDate.AddDays(7);
        var rental = new Rental("rental-1", "driver-1", "moto-1", createdAt, startDate, expectedEnd, expectedEnd, RentalPlanType.Days7);

        // Act
        var cost = rental.CalculateFinalCost(expectedEnd);

        // Assert
        Assert.Equal(210.00m, cost);
    }

    [Fact]
    public void CalculateFinalCost_WhenReturnedEarlyOn7DaysPlan_Applies20PercentPenaltyOnUnusedDays()
    {
        // Arrange: 7-day plan at R$ 30/day. Start is 2026-10-02, Expected is 2026-10-09.
        // Return 2 days early (after 5 days): 5 days used * 30 = 150. Unused 2 days * 30 * 0.20 = 12. Total = 162.
        var createdAt = new DateTime(2026, 10, 1);
        var startDate = createdAt.Date.AddDays(1);
        var expectedEnd = startDate.AddDays(7);
        var rental = new Rental("rental-1", "driver-1", "moto-1", createdAt, startDate, expectedEnd, expectedEnd, RentalPlanType.Days7);
        var returnDate = startDate.AddDays(5);

        // Act
        var cost = rental.CalculateFinalCost(returnDate);

        // Assert
        Assert.Equal(162.00m, cost);
    }

    [Fact]
    public void CalculateFinalCost_WhenReturnedLate_Applies50ReaisPerExtraDay()
    {
        // Arrange: 7-day plan at R$ 30/day = 210. Returned 3 days late: 210 + (3 * 50) = 360.
        var createdAt = new DateTime(2026, 10, 1);
        var startDate = createdAt.Date.AddDays(1);
        var expectedEnd = startDate.AddDays(7);
        var rental = new Rental("rental-1", "driver-1", "moto-1", createdAt, startDate, expectedEnd, expectedEnd, RentalPlanType.Days7);
        var returnDate = expectedEnd.AddDays(3);

        // Act
        var cost = rental.CalculateFinalCost(returnDate);

        // Assert
        Assert.Equal(360.00m, cost);
    }

    [Fact]
    public void CalculateFinalCost_WhenReturnedEarlyOn15DaysPlan_Applies40PercentPenaltyOnUnusedDays()
    {
        // Arrange: 15-day plan at R$ 28/day.
        // Return 5 days early (after 10 days): 10 days used * 28 = 280. Unused 5 days * 28 * 0.40 = 56. Total = 336.
        var createdAt = new DateTime(2026, 10, 1);
        var startDate = createdAt.Date.AddDays(1);
        var expectedEnd = startDate.AddDays(15);
        var rental = new Rental("rental-1", "driver-1", "moto-1", createdAt, startDate, expectedEnd, expectedEnd, RentalPlanType.Days15);
        var returnDate = startDate.AddDays(10);

        // Act
        var cost = rental.CalculateFinalCost(returnDate);

        // Assert
        Assert.Equal(336.00m, cost);
    }

    [Fact]
    public void CompleteReturn_WhenDateIsBeforeStartDate_ThrowsDomainException()
    {
        var createdAt = new DateTime(2026, 10, 1);
        var startDate = createdAt.Date.AddDays(1);
        var expectedEnd = startDate.AddDays(7);
        var rental = new Rental("rental-1", "driver-1", "moto-1", createdAt, startDate, expectedEnd, expectedEnd, RentalPlanType.Days7);
        var invalidReturnDate = startDate.AddDays(-1);

        Assert.Throws<DesafioBackend.Domain.Exceptions.DomainException>(() =>
            rental.CompleteReturn(invalidReturnDate));
    }

    [Fact]
    public void CompleteReturn_WhenAlreadyReturned_ThrowsDomainException()
    {
        var createdAt = new DateTime(2026, 10, 1);
        var startDate = createdAt.Date.AddDays(1);
        var expectedEnd = startDate.AddDays(7);
        var rental = new Rental("rental-1", "driver-1", "moto-1", createdAt, startDate, expectedEnd, expectedEnd, RentalPlanType.Days7);

        rental.CompleteReturn(expectedEnd);

        Assert.Throws<DesafioBackend.Domain.Exceptions.DomainException>(() =>
            rental.CompleteReturn(expectedEnd));
    }
}
