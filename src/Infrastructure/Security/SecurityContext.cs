using Application.Security;

namespace Infrastructure.Security;

public class SecurityContext(IHttpContextAccessor httpContextAccessor) : ISecurityContext
{
    public bool HasPermission(string permission)
    {
        // var permissions = httpContextAccessor.HttpContext?.User.Claims
        //     .Where(c => c.Type == "permission")
        //     .Select(c => c.Value);
        //
        // if (permissions is not null)
        //     return permissions.Contains(permission);
        //
        // return false;
        return true;
    }
}