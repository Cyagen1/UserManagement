using Microsoft.AspNetCore.Mvc;
using UserManagement.Attributes;
using UserManagement.Contracts;
using UserManagement.DataAccess.Repositories;
using UserManagement.Exceptions;
using UserManagement.Services;

namespace UserManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ValidateModel]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IPermissionRepository _permissionRepository;
        private readonly IUserPermissionsService _userPermissionsService;

        public UsersController(IUserRepository userRepository,
            IPermissionRepository permissionRepository,
            IUserPermissionsService userPermissionsService)
        {
            _userRepository = userRepository;
            _permissionRepository = permissionRepository;
            _userPermissionsService = userPermissionsService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsersAsync([FromQuery]string? searchTerm,
            [FromQuery]string? sortColumn,
            [FromQuery]string? sortOrder,
            [FromQuery]int page = 1,
            [FromQuery]int pageSize = 10)
        {
            return Ok(await _userRepository.GetAllUsersAsync(searchTerm, sortColumn, sortOrder, page, pageSize));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserByIdAsync([FromRoute] int id)
        {
            var user = await _userRepository.GetUserById(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUserAsync([FromBody] UserDto userDto)
        {
            var id = await _userRepository.CreateUserAsync(userDto.ToEntity());
            return Ok(id);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserAsync([FromRoute]int id)
        {
            await _userRepository.DeleteUserAsync(id);
            return Accepted();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUserAsync([FromRoute] int id, [FromBody] UserDto userDto)
        {
            var updatedUser = await _userRepository.UpdateUserAsync(userDto.ToEntity(id));
            if (updatedUser != null)
            {
                return NoContent();
            }
            return NotFound();
        }

        [HttpGet("{userId}/permissions")]
        public async Task<IActionResult> GetAllUserPermissions([FromRoute] int userId)
        {
            return Ok(await _permissionRepository.GetAllUserPermissionsAsync(userId));
        }

        [HttpPost("{userId}/permissions/{permissionId}")]
        public async Task<IActionResult> AddUserPermission([FromRoute] int userId, [FromRoute] int permissionId)
        {
            try
            {
                await _userPermissionsService.AddPermissionToUserAsync(userId, permissionId);
            }
            catch (UserNotFoundException)
            {
                return NotFound();
            }
            catch (PermissionNotFoundException)
            {
                return NotFound();
            }
            catch (UserPermissionAlreadyExistsException)
            {
                return Conflict();
            }
            return NoContent();
        }

        [HttpDelete("{userId}/permissions/{permissionId}")]
        public async Task<IActionResult> RemoveUserPermission([FromRoute] int userId, [FromRoute] int permissionId)
        {
            try
            {
                await _userPermissionsService.RemovePermissionFromUserAsync(userId, permissionId);
            }
            catch (UserNotFoundException)
            {
                return NotFound();
            }
            catch (PermissionNotFoundException)
            {
                return NotFound();
            }
            return Accepted();
        }
    }
}
