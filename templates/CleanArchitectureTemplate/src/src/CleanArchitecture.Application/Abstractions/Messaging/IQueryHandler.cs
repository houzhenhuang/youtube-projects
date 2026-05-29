namespace CleanArchitecture.Application.Abstractions.Messaging;

/// <inheritdoc />
public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : IQuery<TResponse>
{
}