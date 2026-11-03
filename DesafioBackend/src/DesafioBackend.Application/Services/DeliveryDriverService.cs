using DesafioBackend.Application.Common.Exceptions;
using DesafioBackend.Application.Common.Interfaces;
using DesafioBackend.Application.DTOs;
using DesafioBackend.Domain.Entities;
using DesafioBackend.Domain.Enums;
using DesafioBackend.Domain.Repositories;

namespace DesafioBackend.Application.Services;

public class DeliveryDriverService : IDeliveryDriverService
{
    private readonly IDeliveryDriverRepository _driverRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IStorageService _storageService;

    public DeliveryDriverService(
        IDeliveryDriverRepository driverRepository,
        IUnitOfWork unitOfWork,
        IStorageService storageService)
    {
        _driverRepository = driverRepository;
        _unitOfWork = unitOfWork;
        _storageService = storageService;
    }

    public async Task CreateAsync(CreateDeliveryDriverRequest request, CancellationToken cancellationToken = default)
    {
        if (request is null)
            throw new ValidationException("Dados do entregador inválidos.");

        if (string.IsNullOrWhiteSpace(request.Identificador) ||
            string.IsNullOrWhiteSpace(request.Nome) ||
            string.IsNullOrWhiteSpace(request.Cnpj) ||
            string.IsNullOrWhiteSpace(request.NumeroCnh) ||
            string.IsNullOrWhiteSpace(request.TipoCnh))
        {
            throw new ValidationException("Todos os campos do entregador são obrigatórios.");
        }

        var driverExists = await _driverRepository.GetByIdAsync(request.Identificador, cancellationToken);
        if (driverExists != null)
        {
            throw new ValidationException("O identificador informado já está cadastrado.");
        }

        var cnpjExists = await _driverRepository.ExistsByCnpjAsync(request.Cnpj, cancellationToken);
        if (cnpjExists)
        {
            throw new ValidationException("O CNPJ informado já está cadastrado.");
        }

        var cnhExists = await _driverRepository.ExistsByCnhNumberAsync(request.NumeroCnh, cancellationToken);
        if (cnhExists)
        {
            throw new ValidationException("O número da CNH informado já está cadastrado.");
        }

        var cnhType = ParseCnhType(request.TipoCnh);

        string? imagePath = null;
        if (!string.IsNullOrWhiteSpace(request.ImagemCnh))
        {
            imagePath = await SaveBase64ImageAsync(request.ImagemCnh, cancellationToken);
        }

        var driver = new DeliveryDriver(
            request.Identificador,
            request.Nome,
            request.Cnpj,
            request.DataNascimento,
            request.NumeroCnh,
            cnhType,
            imagePath
        );

        await _driverRepository.AddAsync(driver, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
    }

    public async Task UpdateCnhImageAsync(string id, UpdateCnhImageRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ValidationException("Identificador inválido.");

        if (request is null || string.IsNullOrWhiteSpace(request.ImagemCnh))
            throw new ValidationException("A imagem da CNH é obrigatória.");

        var driver = await _driverRepository.GetByIdAsync(id, cancellationToken);
        if (driver is null)
        {
            throw new NotFoundException("Entregador não encontrado.");
        }

        var imagePath = await SaveBase64ImageAsync(request.ImagemCnh, cancellationToken);
        driver.UpdateCnhImage(imagePath);

        _driverRepository.Update(driver);
        await _unitOfWork.CommitAsync(cancellationToken);
    }

    public async Task<DeliveryDriverResponse> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ValidationException("Identificador inválido.");

        var driver = await _driverRepository.GetByIdAsync(id, cancellationToken);
        if (driver is null)
        {
            throw new NotFoundException("Entregador não encontrado.");
        }

        return new DeliveryDriverResponse(
            driver.Id,
            driver.Name,
            driver.Cnpj,
            driver.BirthDate,
            driver.CnhNumber,
            driver.CnhType.ToString(),
            driver.CnhImageUrl
        );
    }

    public async Task<IReadOnlyList<DeliveryDriverResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var drivers = await _driverRepository.GetAllAsync(cancellationToken);
        return drivers.Select(driver => new DeliveryDriverResponse(
            driver.Id,
            driver.Name,
            driver.Cnpj,
            driver.BirthDate,
            driver.CnhNumber,
            driver.CnhType.ToString(),
            driver.CnhImageUrl)).ToList();
    }

    private static DriverLicenseType ParseCnhType(string tipo)
    {
        var cleaned = tipo.Trim().ToUpperInvariant().Replace("+", "");
        return cleaned switch
        {
            "A" => DriverLicenseType.A,
            "B" => DriverLicenseType.B,
            "AB" => DriverLicenseType.AB,
            _ => throw new ValidationException("Tipo de CNH inválido. Válidos: A, B ou A+B.")
        };
    }

    private async Task<string> SaveBase64ImageAsync(string base64String, CancellationToken cancellationToken)
    {
        try
        {
            var rawBase64 = base64String;
            if (rawBase64.Contains(','))
            {
                rawBase64 = rawBase64.Substring(rawBase64.IndexOf(',') + 1);
            }

            var imageBytes = Convert.FromBase64String(rawBase64.Trim());
            var (extension, contentType) = DetectImageFormat(imageBytes);

            using var memoryStream = new MemoryStream(imageBytes);
            return await _storageService.SaveFileAsync(memoryStream, $"cnh_{Guid.NewGuid()}{extension}", contentType, cancellationToken);
        }
        catch (FormatException)
        {
            throw new ValidationException("A imagem fornecida não é uma string Base64 válida.");
        }
    }

    private static (string Extension, string ContentType) DetectImageFormat(byte[] bytes)
    {
        if (bytes.Length >= 8 &&
            bytes[0] == 0x89 && bytes[1] == 0x50 && bytes[2] == 0x4E && bytes[3] == 0x47 &&
            bytes[4] == 0x0D && bytes[5] == 0x0A && bytes[6] == 0x1A && bytes[7] == 0x0A)
        {
            return (".png", "image/png");
        }

        if (bytes.Length >= 2 && bytes[0] == 0x42 && bytes[1] == 0x4D)
        {
            return (".bmp", "image/bmp");
        }

        throw new ValidationException("Formato de arquivo inválido. Somente PNG ou BMP são aceitos.");
    }
}

