namespace DesafioBackend.Application.Common.Interfaces;

public interface IMessageBus
{
    Task PublishAsync<T>(T message, CancellationToken cancellationToken = default) where T : class;
}

