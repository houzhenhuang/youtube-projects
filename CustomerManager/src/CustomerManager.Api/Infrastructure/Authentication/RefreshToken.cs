namespace CustomerManager.Api.Infrastructure.Authentication;

public sealed class RefreshToken
{
    public RefreshToken(string token, DateTime expiresOnUtc)
    {
        Token = token;
        ExpiresOnUtc = expiresOnUtc;
    }
    /// <summary>
    /// 获取这个刷新令牌的值。
    /// </summary>
    public string Token { get; set; }

    /// <summary>
    /// 获取UTC格式的到期日期和时间。
    /// </summary>
    public DateTime ExpiresOnUtc { get; set; }

    /// <summary>
    /// 检查刷新令牌是否已过期。
    /// </summary>
    /// <param name="utcNow">UTC格式的当前日期和时间。</param>
    /// <returns>如果刷新令牌已过期，则为True，否则为false。</returns>
    public bool IsExpired(DateTime utcNow) => ExpiresOnUtc < utcNow;
}