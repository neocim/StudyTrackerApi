using Application.Security;

namespace Infrastructure.Security;

public class SecurityContext(
    IHttpContextAccessor httpContextAccessor,
    ILogger<SecurityContext> logger) : ISecurityContext
{
    public bool HasPermission(string permission)
    {
        var hasPermission = httpContextAccessor.HttpContext?.User.Claims
            .Where(c => c.Type == "permissions")
            .Any(c => c.Value == permission) ?? false;

        logger.LogDebug($"User has permission: {hasPermission}");

        return hasPermission;
    }
}