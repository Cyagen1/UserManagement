using System.Linq.Expressions;
using UserManagement.DataAccess.Entities;

namespace UserManagement.DataAccess.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;

        public UserRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<PagedList<User>> GetAllUsersAsync(string? searchTerm, string? sortColumn, string? sortOrder, int page, int pageSize)
        {
            var usersQuery = _context.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                usersQuery = usersQuery.Where(x => x.Username.Contains(searchTerm));
            }

            if (!string.IsNullOrWhiteSpace(sortColumn))
            {
                var sortProperty = GetSortProperty(sortColumn);
                if (!string.IsNullOrWhiteSpace(sortOrder))
                {
                    if (sortOrder.ToLower() == "desc")
                    {
                        usersQuery = usersQuery.OrderByDescending(sortProperty);
                    }
                    else if (sortOrder.ToLower() == "asc")
                    {
                        usersQuery = usersQuery.OrderBy(sortProperty);
                    }
                }
            }

            var users = await PagedList<User>.CreateAsync(usersQuery, page, pageSize);

            return users;
        }
        public async Task<User> GetUserById(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<int> CreateUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user.Id;
        }

        public async Task DeleteUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }

        public async Task<User> UpdateUserAsync(User user)
        {
            var currentUser = await _context.Users.FindAsync(user.Id);
            if (currentUser != null)
            {
                currentUser.Username = user.Username;
                currentUser.Password = user.Password;
                await _context.SaveChangesAsync();
                return currentUser;
            }
            return null;
        }
        private static Expression<Func<User, object>> GetSortProperty(string sortColumn)
        {
            switch (sortColumn.ToLower())
            {
                case "username":
                    return user => user.Username;
                case "status":
                    return user => user.Status;
                default:
                    return user => user.Id;
            }
        }
    }
}
