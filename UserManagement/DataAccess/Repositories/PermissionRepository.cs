using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using UserManagement.DataAccess.Entities;

namespace UserManagement.DataAccess.Repositories
{
    public class PermissionRepository : IPermissionRepository
    {
        private readonly DataContext _context;

        public PermissionRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<PagedList<Permission>> GetAllPermissionsAsync(string? searchTerm, string? sortColumn, string? sortOrder, int page, int pageSize)
        {
            var permissionsQuery = _context.Permissions.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                permissionsQuery = permissionsQuery.Where(x => x.Code.Contains(searchTerm) 
                || x.Description.Contains(searchTerm));
            }

            if (!string.IsNullOrWhiteSpace(sortColumn))
            {
                var sortProperty = GetSortProperty(sortColumn);
                if (!string.IsNullOrWhiteSpace(sortOrder))
                {
                    if (sortOrder.ToLower() == "desc")
                    {
                        permissionsQuery = permissionsQuery.OrderByDescending(sortProperty);
                    }
                    else if (sortOrder.ToLower() == "asc")
                    {
                        permissionsQuery = permissionsQuery.OrderBy(sortProperty);
                    }
                }
            }

            var permissions = await PagedList<Permission>.CreateAsync(permissionsQuery, page, pageSize);

            return permissions;
        }

        public async Task<IEnumerable<Permission>> GetAllUserPermissionsAsync(int userId)
        {
            var userPermissions = await _context.UserPermissions
            .Where(x => x.UserId == userId)
            .Include(x => x.Permission)
            .Select(x => x.Permission)
            .ToListAsync();

            return userPermissions;
        }
        public async Task<Permission> GetPermissionById(int id)
        {
            return await _context.Permissions.FindAsync(id);
        }

        public async Task<int> CreatePermissionAsync(Permission permission)
        {
            await _context.Permissions.AddAsync(permission);
            await _context.SaveChangesAsync();
            return permission.Id;
        }

        public async Task DeletePermissionAsync(int id)
        {
            var permission = await _context.Permissions.FindAsync(id);
            _context.Permissions.Remove(permission);
            await _context.SaveChangesAsync();
        }

        public async Task<Permission> UpdatePermissionAsync(Permission permission)
        {
            var currentPermission = await _context.Permissions.FindAsync(permission.Id);
            if (currentPermission != null)
            {
                currentPermission.Code = permission.Code;
                currentPermission.Description = permission.Description;
                await _context.SaveChangesAsync();
                return currentPermission;
            }
            return null;
        }
        private static Expression<Func<Permission, object>> GetSortProperty(string sortColumn)
        {
            switch (sortColumn.ToLower())
            {
                case "code":
                    return permission => permission.Code;
                case "description":
                    return permission => permission.Description;
                default:
                    return permission => permission.Id;
            }
        }
    }
}
