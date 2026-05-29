namespace CleanArchitecture.Domain.Primitives;

public abstract class ValueObject : IEquatable<ValueObject>
{
    public override bool Equals(object? obj)
    {
        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override string ToString()
    {
        return base.ToString();
    }

    public bool Equals(ValueObject? other)
    {
        return base.Equals(other);
    }
}