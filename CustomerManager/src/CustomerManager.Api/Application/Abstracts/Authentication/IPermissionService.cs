namespace CustomerManager.Api.Application.Abstracts.Authentication;

public interface IPermissionService
{
    Task<HashSet<string>> GetPermissionsAsync(int userId);
}
