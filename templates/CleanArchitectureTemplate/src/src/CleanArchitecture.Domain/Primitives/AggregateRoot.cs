namespace CleanArchitecture.Domain.Primitives;

public interface IAggregateRoot
{
    IReadOnlyCollection<DomainEvent> DomainEvents { get; }

    void Raise(DomainEvent domainEvent);

    void RemoveDomainEvent(DomainEvent domainEvent);

    void ClearDomainEvents();
}

/// <summary>
/// 聚合根
/// </summary>
public abstract class AggregateRoot<TKey> : Entity<TKey>, IAggregateRoot
    where TKey : class
{
    protected AggregateRoot(TKey id) : base(id)
    {
        
    }

    protected AggregateRoot()
    {
        
    }

    private readonly List<DomainEvent> _domainEvents = new();

    public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void Raise(DomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void RemoveDomainEvent(DomainEvent domainEvent)
    {
        _domainEvents?.Remove(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}