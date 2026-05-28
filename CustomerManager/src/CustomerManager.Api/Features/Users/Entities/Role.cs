namespace CustomerManager.Api.Features.Users.Entities;

public class Role
{
    public static readonly Role Registered = new(1, "Registered");
    public Role(int id, string name)
    {
        Id = id;
        Name = name;
    }
    public int Id { get; set; }
    public string Name { get; set; }

    public ICollection<Permission> Permissions { get; set; }

    public static IEnumerable<Role> GetValues()
    {
        yield return Registered;
    }
}
