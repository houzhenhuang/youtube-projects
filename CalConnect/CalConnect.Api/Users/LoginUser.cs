using CalConnect.Api.Database;
using CalConnect.Api.Users.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace CalConnect.Api.Users;

internal sealed class LoginUser(ApplicationDbContext context, PasswordHasher passwordHasher, TokenProvider tokenProvider)
{
    public record Request(string Email, string Password);

    public async Task<string> Handle(Request request)
    {
        var user = await context.Users.SingleOrDefaultAsync(u => u.Email == request.Email);
        if (user is null || !user.EmailVerified)
        {
            throw new Exception("用户不存在");
        }

        bool verified = passwordHasher.Verify(request.Password, user.PasswordHash);
        if (!verified)
        {
            throw new Exception("密码错误");
        }

        string token = tokenProvider.Create(user);

        return token;
    }
}