using Application.Security;

namespace Infrastructure.Security;

public class SecurityContext(
    IHttpContextAccessor httpContextAccessor,
    ILogger<SecurityContext> logger) : ISecurityContext
{
    public bool HasPermission(string permission)
    {
        var permissions = httpContextAccessor.HttpContext?.User.Claims
            .Where(c => c.Type == "permission")
            .Select(c => c.Value);

        if (permissions is not null)
        {
            logger.LogDebug($"User has permission `{permission}`");
            return permissions.Contains(permission);
        }

        logger.LogError($"User does not have permission `{permission}`");

        return false;
    }
}