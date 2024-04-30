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
    public class PermissionsController : ControllerBase
    {
        private readonly IPermissionRepository _permissionRepository;
        private readonly IMapper _mapper;

        public PermissionsController(IPermissionRepository permissionRepository, IMapper mapper)
        {
            _permissionRepository = permissionRepository;
            _mapper = mapper;
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
            var permission = _mapper.Map<Permission>(permissionDto);
            var id = await _permissionRepository.CreatePermissionAsync(permission);
            return Ok(id);
        }

        [HttpDelete]
        public async Task<IActionResult> DeletePermissionAsync(int id)
        {
            await _permissionRepository.DeletePermissionAsync(id);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> UpdatePermissionAsync([FromBody] PermissionDto permissionDto)
        {
            var updatedPermission = await _permissionRepository.UpdatePermissionAsync(_mapper.Map<Permission>(permissionDto));
            if (updatedPermission != null)
            {
                return Ok(updatedPermission);
            }
            return NotFound();
        }
    }
}
