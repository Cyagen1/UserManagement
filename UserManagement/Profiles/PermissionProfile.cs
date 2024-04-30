using AutoMapper;
using UserManagement.Contracts;
using UserManagement.DataAccess.Entities;

namespace UserManagement.Profiles
{
    public class PermissionProfile : Profile
    {
        public PermissionProfile()
        {
            CreateMap<PermissionDto, Permission>();
        }
    }
}
