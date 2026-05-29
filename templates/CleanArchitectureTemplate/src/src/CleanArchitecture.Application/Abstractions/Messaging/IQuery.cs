namespace CleanArchitecture.Application.Abstractions.Messaging;

/// <inheritdoc />
public interface IQuery<TResponse> : IRequest<Result<TResponse>>
{
}