using UserService.Models;

namespace UserService.Services
{
    public class UserService : IUserService
    {
        private readonly List<User> _users = new()
        {
            new User { Id = 1, FirstName = "Json", LastName = "Dev",
                       Email = "json@example.com", PhoneNumber = "5555555555" },
            new User { Id = 2, FirstName = "Clara", LastName = "Dev",
                   Email = "clara@example.com", PhoneNumber = "5555555544" }
          };
        private int _nextId = 3;

        public Task<List<User>> GetAllAsync()
        {
            return Task.FromResult(_users.ToList());
        }

        public  Task<User?> GetByIdAsync(int id)
        {
            var user =  _users.FirstOrDefault(u => u.Id == id);
            return Task.FromResult(user);
        }

        public Task<(User? User, string? Error)> CreateAsync(CreateUserRequest request)
        {
            var emailExist = _users.Any(u => u.Email == request.Email);
            if (emailExist)
                return Task.FromResult<(User?, string?)>((null, "Email already exist"));

            var user = new User
            {
                Id = _nextId,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                CreatedAt = DateTime.UtcNow
            };

            _users.Add(user);
            return Task.FromResult<(User?, string?)>((user, null));
        }

        public Task<User?> UpdateAsync(int id, CreateUserRequest request)
        {
            var user = _users.FirstOrDefault(u => u.Id == id);
            if (user is null) return Task.FromResult<User?>(null);

            
            var emailTaken = _users.Any(u => u.Email == request.Email && u.Id != id);
            if (emailTaken) return Task.FromResult<User?>(null);

            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.Email = request.Email;
            user.PhoneNumber = request.PhoneNumber;

            return Task.FromResult<User?>(user);
        }

        public Task<bool> DeleteAsync(int id)
        {
            var user = _users.FirstOrDefault(u => u.Id == id);
            if (user is null) return Task.FromResult(false);


            _users.Remove(user);
            return Task.FromResult(true);
        }

    }
}
