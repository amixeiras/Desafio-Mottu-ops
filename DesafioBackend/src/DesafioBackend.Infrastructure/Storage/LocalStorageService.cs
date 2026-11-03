using DesafioBackend.Application.Common.Exceptions;
using DesafioBackend.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;

namespace DesafioBackend.Infrastructure.Storage;

public class LocalStorageService : IStorageService
{
    private readonly string _storageBasePath;

    public LocalStorageService(IConfiguration configuration)
    {
        var configuredPath = configuration["Storage:BasePath"];
        _storageBasePath = !string.IsNullOrWhiteSpace(configuredPath) 
            ? configuredPath 
            : Path.Combine(AppContext.BaseDirectory, "uploads", "cnh");

        if (!Directory.Exists(_storageBasePath))
        {
            Directory.CreateDirectory(_storageBasePath);
        }
    }

    public async Task<string> SaveFileAsync(Stream fileStream, string fileName, string contentType, CancellationToken cancellationToken = default)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        if (extension != ".png" && extension != ".bmp")
        {
            throw new ValidationException("O formato do arquivo de imagem deve ser PNG ou BMP.");
        }

        var uniqueFileName = $"{Guid.NewGuid()}{extension}";
        var destinationPath = Path.Combine(_storageBasePath, uniqueFileName);

        using (var destStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write, FileShare.None))
        {
            await fileStream.CopyToAsync(destStream, cancellationToken);
        }

        return destinationPath;
    }

    public Task<Stream?> GetFileAsync(string fileKey, CancellationToken cancellationToken = default)
    {
        var filePath = Path.Combine(_storageBasePath, Path.GetFileName(fileKey));
        if (!File.Exists(filePath))
        {
            return Task.FromResult<Stream?>(null);
        }

        Stream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        return Task.FromResult<Stream?>(stream);
    }
}

