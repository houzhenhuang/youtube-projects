using CustomerManager.Api.Features.Users.Entities;
using CustomerManager.Api.Infrastructure.Authentication;

namespace CustomerManager.Api.Application.Abstracts.Authentication;

public interface IJwtProvider
{
    /// <summary>
    /// 获取指定用户的访问令牌。
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    Task<AccessTokens> GetAccessTokens(User user);
}
