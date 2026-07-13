namespace Notes.Api.Domain;

public class User : ITenantOwned
{
    public Guid Id { get; set; }

    public Guid TenantId { get; set; }
    public string Email { get; set; } = string.Empty;
}
