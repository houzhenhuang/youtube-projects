namespace Notes.Api.Domain;

public interface ITenantOwned
{
    Guid TenantId { get; set; }
}
