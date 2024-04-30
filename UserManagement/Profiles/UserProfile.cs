using AutoMapper;
using UserManagement.Contracts;
using UserManagement.DataAccess.Entities;

namespace UserManagement.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<UserDto, User>();
        }
    }
}
