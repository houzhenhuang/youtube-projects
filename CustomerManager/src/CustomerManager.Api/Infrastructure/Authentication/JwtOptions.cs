namespace CustomerManager.Api.Infrastructure.Authentication;

/// <summary>
/// 表示 JWT 配置设置。
/// </summary>
public class JwtOptions
{
    /// <summary>
    /// 获取发行人。
    /// </summary>
    public string Issuer { get; init; } = default!;

    /// <summary>
    /// 获取受众。
    /// </summary>
    public string Audience { get; init; } = default!;

    /// <summary>
    /// 获取安全密钥。
    /// </summary>
    public string SecurityKey { get; init; } = default!;

    /// <summary>
    /// 获取令牌过期时间（以分钟为单位）。
    /// </summary>
    public int AccessTokenExpirationInMinutes { get; init; }

    /// <summary>
    /// 获取刷新令牌的过期时间（以分钟为单位）。
    /// </summary>
    public int RefreshTokenExpirationInMinutes { get; init; }
}
