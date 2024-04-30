namespace UserManagement.Services
{
    public interface IUserPermissionsService
    {
        Task AddPermissionToUserAsync(int userId, int permissionId);
        Task RemovePermissionFromUserAsync(int userId, int permissionId);
    }
}
