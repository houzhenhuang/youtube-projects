namespace CalConnect.Api.Users.Infrastructure;

public interface IUserRepository
{
    Task<bool> Exists(string email);

    Task Insert(User user);

    Task<User?> GetByEmail(string email);
}