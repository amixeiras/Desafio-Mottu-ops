namespace DesafioBackend.Application.Common.Interfaces;

public interface IStorageService
{
    Task<string> SaveFileAsync(Stream fileStream, string fileName, string contentType, CancellationToken cancellationToken = default);
    Task<Stream?> GetFileAsync(string fileKey, CancellationToken cancellationToken = default);
}

