namespace Application.Security;

/// Dont use it now
public interface ISecurityContext
{
    public bool HasPermission(string permission);
}