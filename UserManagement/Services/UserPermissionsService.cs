using Microsoft.EntityFrameworkCore;
using UserManagement.DataAccess;
using UserManagement.DataAccess.Entities;
using UserManagement.DataAccess.Repositories;
using UserManagement.Exceptions;

namespace UserManagement.Services
{
    public class UserPermissionsService : IUserPermissionsService
    {
        private readonly DataContext _context;
        private readonly IUserRepository _userRepository;
        private readonly IPermissionRepository _permissionRepository;

        public UserPermissionsService(DataContext context, IUserRepository userRepository, IPermissionRepository permissionRepository)
        {
            _context = context;
            _userRepository = userRepository;
            _permissionRepository = permissionRepository;
        }

        public async Task AddPermissionToUserAsync(int userId, int permissionId)
        {
            await EnsureUserAndPermissionExistAsync(userId, permissionId);
            if (await _context.UserPermissions.Where(x => x.UserId == userId && x.PermissionId == permissionId).AnyAsync())
            {
                throw new UserPermissionAlreadyExistsException(userId, permissionId);
            }

            var userPermission = new UserPermission
            {
                UserId = userId,
                PermissionId = permissionId
            };
            await _context.UserPermissions.AddAsync(userPermission);
            await _context.SaveChangesAsync();
        }

        public async Task RemovePermissionFromUserAsync(int userId, int permissionId)
        {
            await EnsureUserAndPermissionExistAsync(userId, permissionId);
            var userPermission = await _context.UserPermissions.Where(x => x.UserId == userId && x.PermissionId == permissionId)
                .FirstOrDefaultAsync();
            if (userPermission == null)
            {
                return;
            }

            _context.UserPermissions.Remove(userPermission);
            await _context.SaveChangesAsync();
        }

        private async Task EnsureUserAndPermissionExistAsync(int userId, int permissionId)
        {
            if (await _userRepository.GetUserById(userId) == null)
            {
                throw new UserNotFoundException(userId);
            }

            if (await _permissionRepository.GetPermissionById(permissionId) == null)
            {
                throw new PermissionNotFoundException(permissionId);
            }
        }
    }
}
