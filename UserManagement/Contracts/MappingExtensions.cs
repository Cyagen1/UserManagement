using UserManagement.DataAccess.Entities;

namespace UserManagement.Contracts
{
    public static class MappingExtensions
    {
        public static User ToEntity(this UserDto dto)
            => new()
            {
                Password = dto.Password,
                Status = dto.Status,
                Username = dto.Username,
            };

        public static User ToEntity(this UserDto dto, int id)
            => new()
            {
                Id = id,
                Password = dto.Password,
                Status = dto.Status,
                Username = dto.Username,
            };

        public static Permission ToEntity(this PermissionDto dto)
            => new()
            {
                Code = dto.Code,
                Description = dto.Description,
            };

        public static Permission ToEntity(this PermissionDto dto, int id)
            => new()
            {
                Id = id,
                Code = dto.Code,
                Description = dto.Description,
            };
    }
}
