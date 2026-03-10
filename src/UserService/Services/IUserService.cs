using UserService.Models;

namespace UserService.Services
{
    public interface IUserService
    {

        Task<List<User>> GetAllAsync();
        Task<User?> GetByIdAsync(int id);
        Task<(User? User, string? Error)> CreateAsync(CreateUserRequest request);
        Task<User?> UpdateAsync(int id, CreateUserRequest request);
        Task<bool> DeleteAsync(int id);
    }
}
