using CleanArchitecture.Domain.AggregatesEntities.ProductAggregate;

namespace CleanArchitecture.Domain.DomainEvents;

public record ProductCreatedDomainEvent(Guid Id, ProductId ProductId) : DomainEvent(Id);