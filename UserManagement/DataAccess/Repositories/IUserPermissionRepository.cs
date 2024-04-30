namespace UserManagement.DataAccess.Repositories
{
    public interface IUserPermissionRepository
    {
        Task AddUserPermissionAsync(int userId, int permissionId);
        Task RemoveUserPermissionAsync(int userId, int permissionId);
    }
}
