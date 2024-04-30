using UserManagement.DataAccess.Entities;

namespace UserManagement.DataAccess.Repositories
{
    public interface IUserRepository
    {
        Task<PagedList<User>> GetAllUsersAsync(string? searchTerm, string? sortColumn, string? sortOrder, int page, int pageSize);
        Task<User> GetUserById(int id);
        Task<int> CreateUserAsync(User user);
        Task DeleteUserAsync(int id);
        Task<User> UpdateUserAsync(User user);
    }
}
