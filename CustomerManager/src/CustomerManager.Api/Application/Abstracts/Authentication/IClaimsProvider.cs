using CustomerManager.Api.Features.Users.Entities;
using System.Security.Claims;

namespace CustomerManager.Api.Application.Abstracts.Authentication;

/// <summary>
/// claims 提供者
/// </summary>
public interface IClaimsProvider
{
    /// <summary>
    /// 获取指定用户的 claims。
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    Task<IEnumerable<Claim>> GetClaimsForUser(User user);
}
