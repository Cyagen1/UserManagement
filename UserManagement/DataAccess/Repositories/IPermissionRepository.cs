using UserManagement.DataAccess.Entities;

namespace UserManagement.DataAccess.Repositories
{
    public interface IPermissionRepository
    {
        Task<PagedList<Permission>> GetAllPermissionsAsync(string? searchTerm, string? sortColumn, string? sortOrder, int page, int pageSize);
        Task<IEnumerable<Permission>> GetAllUserPermissionsAsync(int userId);
        Task<Permission> GetPermissionById(int id);
        Task<int> CreatePermissionAsync(Permission permission);
        Task DeletePermissionAsync(int id);
        Task<Permission> UpdatePermissionAsync(Permission permission);
    }
}
