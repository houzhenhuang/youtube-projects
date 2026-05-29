using CleanArchitecture.Utility.Primitives.Errors;

namespace CleanArchitecture.Domain.Exceptions;

/// <summary>
/// 领域异常
/// </summary>
public sealed class DomainException : Exception
{
    public DomainException()
    { }

    public DomainException(Error error)
        : base(error.Message)
    { }

    public DomainException(Error error, Exception innerException)
        : base(error.Message, innerException)
    { }
    
    /// <summary>
    /// Gets the error.
    /// </summary>
    public Error Error { get; }
}