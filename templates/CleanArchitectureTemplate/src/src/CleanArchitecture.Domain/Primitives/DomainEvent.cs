namespace CleanArchitecture.Domain.Primitives;

public record DomainEvent(Guid Id) : INotification;