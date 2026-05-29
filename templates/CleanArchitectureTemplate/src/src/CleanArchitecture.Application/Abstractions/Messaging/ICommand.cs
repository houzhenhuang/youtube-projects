namespace CleanArchitecture.Application.Abstractions.Messaging;

/// <inheritdoc />
public interface ICommand<out TResponse> : IRequest<TResponse>
    where TResponse : Result
{
}

/// <inheritdoc />
public interface ICommand : IRequest<Result>
{
}