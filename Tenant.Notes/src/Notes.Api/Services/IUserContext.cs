namespace Notes.Api.Services;

public interface IUserContext
{
    Guid UserId { get; }

    Task<Guid> GetTenantId();
}
