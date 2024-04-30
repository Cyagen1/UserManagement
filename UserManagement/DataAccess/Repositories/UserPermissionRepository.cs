using Microsoft.EntityFrameworkCore;
using UserManagement.DataAccess.Entities;

namespace UserManagement.DataAccess.Repositories
{
    public class UserPermissionRepository : IUserPermissionRepository
    {
        private readonly DataContext _context;

        public UserPermissionRepository(DataContext context)
        {
            _context = context;
        }

        public async Task AddUserPermissionAsync(int userId, int permissionId)
        {
            var userPermission = new UserPermission
            {
                UserId = userId,
                PermissionId = permissionId
            };
            await _context.UserPermissions.AddAsync(userPermission);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveUserPermissionAsync(int userId, int permissionId)
        {
            var userPermission = await _context.UserPermissions.Where(x => x.UserId == userId && x.PermissionId == permissionId)
                .FirstOrDefaultAsync();
            _context.UserPermissions.Remove(userPermission);
            await _context.SaveChangesAsync();
        }
    }
}
