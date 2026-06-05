using CalConnect.Api.Users.Infrastructure;

namespace CalConnect.Api.Users;

public sealed class LoginUser(IUserRepository userRepository, PasswordHasher passwordHasher)
{
    public record Request(string Email, string Password);

    public async Task<User> Handle(Request request)
    {
        var user = await userRepository.GetByEmail(request.Email);
        if (user is null || !user.EmailVerified)
        {
            throw new Exception("用户不存在");
        }

        bool verified = passwordHasher.Verify(request.Password, user.PasswordHash);
        if (!verified)
        {
            throw new Exception("密码错误");
        }
        return user;
    }
}