using DesafioBackend.Domain.Common;
using DesafioBackend.Domain.Enums;
using DesafioBackend.Domain.Exceptions;

namespace DesafioBackend.Domain.Entities;

public class Rental : BaseEntity<string>, IAggregateRoot
{
    public string DriverId { get; private set; } = string.Empty;
    public string MotorcycleId { get; private set; } = string.Empty;
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public DateTime ExpectedEndDate { get; private set; }
    public DateTime? ReturnDate { get; private set; }
    public RentalPlanType Plan { get; private set; }
    public decimal DailyRate { get; private set; }
    public decimal TotalCost { get; private set; }

    // For EF Core
    protected Rental() { }

    public Rental(string id, string driverId, string motorcycleId, DateTime createdAt, DateTime startDate, DateTime endDate, DateTime expectedEndDate, RentalPlanType plan)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new DomainException("Identificador inválido.");
        if (string.IsNullOrWhiteSpace(driverId))
            throw new DomainException("Entregador inválido.");
        if (string.IsNullOrWhiteSpace(motorcycleId))
            throw new DomainException("Moto inválida.");

        var utcCreatedAt = createdAt.Kind == DateTimeKind.Utc ? createdAt : DateTime.SpecifyKind(createdAt, DateTimeKind.Utc);
        var utcStartDate = startDate.Kind == DateTimeKind.Utc ? startDate : DateTime.SpecifyKind(startDate, DateTimeKind.Utc);
        var utcEndDate = endDate.Kind == DateTimeKind.Utc ? endDate : DateTime.SpecifyKind(endDate, DateTimeKind.Utc);
        var utcExpectedEndDate = expectedEndDate.Kind == DateTimeKind.Utc ? expectedEndDate : DateTime.SpecifyKind(expectedEndDate, DateTimeKind.Utc);

        // O início da locação obrigatoriamente é o primeiro dia após a criação
        var expectedStartDate = utcCreatedAt.Date.AddDays(1);
        if (utcStartDate.Date != expectedStartDate)
        {
            throw new DomainException("A data de início da locação deve ser o primeiro dia após a data de criação.");
        }

        Id = id;
        DriverId = driverId;
        MotorcycleId = motorcycleId;
        Plan = plan;
        DailyRate = GetDailyRate(plan);
        CreatedAt = utcCreatedAt;
        StartDate = utcStartDate;
        EndDate = utcEndDate;
        ExpectedEndDate = utcExpectedEndDate;
        TotalCost = (int)plan * DailyRate;
    }

    public decimal CalculateFinalCost(DateTime returnDate)
    {
        var utcReturnDate = returnDate.Kind == DateTimeKind.Utc ? returnDate : DateTime.SpecifyKind(returnDate, DateTimeKind.Utc);
        if (utcReturnDate.Date < StartDate.Date)
        {
            throw new DomainException("A data de devolução não pode ser anterior à data de início da locação.");
        }

        var planDays = (int)Plan;
        var dailyRate = DailyRate > 0 ? DailyRate : GetDailyRate(Plan);
        var baseTotal = planDays * dailyRate;

        var returnDay = utcReturnDate.Date;
        var expectedEndDay = ExpectedEndDate.Date;

        if (returnDay == expectedEndDay)
        {
            return baseTotal;
        }

        if (returnDay < expectedEndDay)
        {
            // Devolução antecipada: diárias efetivadas + multa sobre não efetivadas
            var usedDays = Math.Max(0, (int)(returnDay - StartDate.Date).TotalDays);
            var unusedDays = Math.Max(0, planDays - usedDays);
            var penaltyPercentage = Plan switch
            {
                RentalPlanType.Days7 => 0.20m,
                RentalPlanType.Days15 => 0.40m,
                _ => 0.00m
            };

            var usedAmount = usedDays * dailyRate;
            var penaltyAmount = unusedDays * dailyRate * penaltyPercentage;
            return usedAmount + penaltyAmount;
        }
        else
        {
            // Devolução tardia: valor base + R$ 50 por diária adicional
            var additionalDays = Math.Max(0, (int)(returnDay - expectedEndDay).TotalDays);
            const decimal additionalDailyRate = 50.00m;
            return baseTotal + (additionalDays * additionalDailyRate);
        }
    }

    public decimal CompleteReturn(DateTime returnDate)
    {
        if (ReturnDate.HasValue)
        {
            throw new DomainException("Esta locação já foi devolvida e finalizada.");
        }

        var utcReturnDate = returnDate.Kind == DateTimeKind.Utc ? returnDate : DateTime.SpecifyKind(returnDate, DateTimeKind.Utc);
        if (utcReturnDate.Date < StartDate.Date)
        {
            throw new DomainException("A data de devolução não pode ser anterior à data de início da locação.");
        }

        ReturnDate = utcReturnDate;
        TotalCost = CalculateFinalCost(utcReturnDate);
        UpdatedAt = DateTime.UtcNow;
        return TotalCost;
    }

    public static decimal GetDailyRate(RentalPlanType plan) => plan switch
    {
        RentalPlanType.Days7 => 30.00m,
        RentalPlanType.Days15 => 28.00m,
        RentalPlanType.Days30 => 22.00m,
        RentalPlanType.Days45 => 20.00m,
        RentalPlanType.Days50 => 18.00m,
        _ => throw new DomainException("Plano de locação inválido.")
    };
}
