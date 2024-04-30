using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Attributes;
using UserManagement.Contracts;
using UserManagement.DataAccess.Entities;
using UserManagement.DataAccess.Repositories;

namespace UserManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ValidateModel]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IPermissionRepository _permissionRepository;
        private readonly IUserPermissionRepository _userPermissionRepository;
        private readonly IMapper _mapper;

        public UsersController(IUserRepository userRepository,
            IPermissionRepository permissionRepository,
            IUserPermissionRepository userPermissionRepository,
            IMapper mapper)
        {
            _userRepository = userRepository;
            _permissionRepository = permissionRepository;
            _userPermissionRepository = userPermissionRepository;
            _mapper = mapper;
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
            var user = _mapper.Map<User>(userDto);
            var id = await _userRepository.CreateUserAsync(user);
            return Ok(id);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteUserAsync(int id)
        {
            await _userRepository.DeleteUserAsync(id);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUserAsync([FromBody] UserDto userDto)
        {
            var updatedUser = await _userRepository.UpdateUserAsync(_mapper.Map<User>(userDto));
            if (updatedUser != null)
            {
                return Ok(updatedUser);
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
            await _userPermissionRepository.AddUserPermissionAsync(userId, permissionId);
            return Ok();
        }

        [HttpDelete("{userId}/permissions/{permissionId}")]
        public async Task<IActionResult> RemoveUserPermission([FromRoute] int userId, [FromRoute] int permissionId)
        {
            await _userPermissionRepository.RemoveUserPermissionAsync(userId, permissionId);
            return Ok();
        }
    }
}
