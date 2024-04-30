namespace UserManagement.Exceptions
{
    public class UserPermissionAlreadyExistsException(int userId, int permissionId)
        : Exception($"User with id {userId} already has permission with id {permissionId}.")
    {
    }
}
