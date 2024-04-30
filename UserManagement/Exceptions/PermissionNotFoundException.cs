namespace UserManagement.Exceptions
{
    public class PermissionNotFoundException(int permissionId)
        : Exception($"Permission with id {permissionId} not found.")
    {
    }
}
