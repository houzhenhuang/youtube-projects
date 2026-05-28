namespace CustomerManager.Api.Infrastructure.Authentication;

/// <summary>
/// 
/// </summary>
/// <param name="Token">访问令牌</param>
/// <param name="RefreshToken"></param>
public sealed record AccessTokens(string Token, RefreshToken RefreshToken);
