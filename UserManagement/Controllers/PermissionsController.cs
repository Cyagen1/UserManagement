using Microsoft.AspNetCore.Mvc;
using UserManagement.Attributes;
using UserManagement.Contracts;
using UserManagement.DataAccess.Repositories;

namespace UserManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ValidateModel]
    public class PermissionsController : ControllerBase
    {
        private readonly IPermissionRepository _permissionRepository;

        public PermissionsController(IPermissionRepository permissionRepository)
        {
            _permissionRepository = permissionRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPermissionsAsync([FromQuery] string? searchTerm,
            [FromQuery] string? sortColumn,
            [FromQuery] string? sortOrder,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            return Ok(await _permissionRepository.GetAllPermissionsAsync(searchTerm, sortColumn, sortOrder, page, pageSize));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPermissionByIdAsync([FromRoute] int id)
        {
            var permission = await _permissionRepository.GetPermissionById(id);
            if (permission == null)
            {
                return NotFound();
            }
            return Ok(permission);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePermissionAsync([FromBody] PermissionDto permissionDto)
        {
            var id = await _permissionRepository.CreatePermissionAsync(permissionDto.ToEntity());
            return Ok(id);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePermissionAsync([FromRoute]int id)
        {
            await _permissionRepository.DeletePermissionAsync(id);
            return Accepted();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePermissionAsync([FromRoute] int id, [FromBody] PermissionDto permissionDto)
        {
            var updatedPermission = await _permissionRepository.UpdatePermissionAsync(permissionDto.ToEntity(id));
            if (updatedPermission != null)
            {
                return NoContent();
            }
            return NotFound();
        }
    }
}
