namespace CleanArchitecture.Domain.Primitives;

public abstract class Entity
{
    public static bool operator ==(Entity? left, Entity? right)
    {
        if (left is null && right is null)
        {
            return true;
        }

        if (left is null || right is null)
        {
            return false;
        }

        return left.Equals(right);
    }

    public static bool operator !=(Entity left, Entity right)
    {
        return !(left == right);
    }
}

/// <summary>
/// 表示所有实体派生自的基类。
/// </summary>
public abstract class Entity<TKey> : Entity
    where TKey : class
{
    public TKey Id { get; protected init; }

    protected Entity(TKey id)
    {
        Id = id;
    }

    protected Entity()
    {
    }

    public override bool Equals(object? obj)
    {
        if (obj == null)
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj is not Entity<TKey> other)
        {
            return false;
        }

        return Id == other.Id;
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode() ^ 32;
    }
}