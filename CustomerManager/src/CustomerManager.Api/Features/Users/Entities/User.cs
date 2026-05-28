namespace CustomerManager.Api.Features.Users.Entities;

public class User
{
    public static readonly User DefaultUser = new()
    {
        Id = 1,
        FirstName = "curitis",
        LastName = "huang",
        Email = "curitis@mogul-tech.com",
        PasswordHash = "123456"
    };

    public int Id { get; set; }

    /// <summary>
    /// Gets the first name.
    /// </summary>
    public string FirstName { get; set; } = default!;

    /// <summary>
    /// Gets the last name.
    /// </summary>
    public string LastName { get; set; } = default!;

    /// <summary>
    /// Gets the email.
    /// </summary>
    public string Email { get; set; } = default!;

    /// <summary>
    /// Gets the password hash.
    /// </summary>
    public string PasswordHash { get; set; } = default!;

    /// <summary>
    /// 
    /// </summary>
    public ICollection<Role> Roles { get; set; }

    public static IEnumerable<User> GetValues()
    {
        yield return DefaultUser;
    }
}
