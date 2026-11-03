using DesafioBackend.Domain.Common;
using DesafioBackend.Domain.Enums;
using DesafioBackend.Domain.Exceptions;

namespace DesafioBackend.Domain.Entities;

public class DeliveryDriver : BaseEntity<string>, IAggregateRoot
{
    public string Name { get; private set; } = string.Empty;
    public string Cnpj { get; private set; } = string.Empty;
    public DateTime BirthDate { get; private set; }
    public string CnhNumber { get; private set; } = string.Empty;
    public DriverLicenseType CnhType { get; private set; }
    public string? CnhImageUrl { get; private set; }

    // For EF Core
    protected DeliveryDriver() { }

    public DeliveryDriver(string id, string name, string cnpj, DateTime birthDate, string cnhNumber, DriverLicenseType cnhType, string? cnhImageUrl = null)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new DomainException("Identificador inválido.");
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Nome é obrigatório.");
        if (string.IsNullOrWhiteSpace(cnpj))
            throw new DomainException("CNPJ é obrigatório.");
        if (string.IsNullOrWhiteSpace(cnhNumber))
            throw new DomainException("Número da CNH é obrigatório.");

        Id = id;
        Name = name.Trim();
        Cnpj = cnpj.Trim();
        BirthDate = birthDate;
        CnhNumber = cnhNumber.Trim();
        CnhType = cnhType;
        CnhImageUrl = cnhImageUrl;
    }

    public bool CanRentMotorcycle() => CnhType == DriverLicenseType.A || CnhType == DriverLicenseType.AB;

    public void UpdateCnhImage(string imageUrl)
    {
        if (string.IsNullOrWhiteSpace(imageUrl))
            throw new DomainException("Caminho da imagem é inválido.");

        CnhImageUrl = imageUrl;
        UpdatedAt = DateTime.UtcNow;
    }
}

