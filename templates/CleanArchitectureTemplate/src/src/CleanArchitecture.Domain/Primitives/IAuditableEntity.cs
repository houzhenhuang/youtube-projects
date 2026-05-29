namespace CleanArchitecture.Domain.Primitives;

/// <summary>
/// 表示可审计实体的标记接口。
/// </summary>
public interface IAuditableEntity
{
    /// <summary>
    /// Gets the created on date and time in UTC format.
    /// </summary>
    DateTime CreatedOnUtc { get; }

    /// <summary>
    /// Gets the modified on date and time in UTC format.
    /// </summary>
    DateTime? ModifiedOnUtc { get; }
}