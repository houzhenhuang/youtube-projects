namespace CustomerManager.Api.Endpoints;

/// <summary>
/// 
/// </summary>
public interface IEndpoint
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="app"></param>
    void MapEndpoint(IEndpointRouteBuilder app);
}
