using Microsoft.EntityFrameworkCore;
using UserService.Data;
using UserService.Models;

namespace UserService.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _context;

    public UserService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<User>> GetAllAsync()
    {
        return await _context.Users.ToListAsync();
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task<(User? User, string? Error)> CreateAsync(CreateUserRequest request)
    {
        // Email kontrolü
        var emailExists = await _context.Users
            .AnyAsync(u => u.Email == request.Email);

        if (emailExists)
            return (null, "Email already in use.");

        var user = new User
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return (user, null);
    }

    public async Task<User?> UpdateAsync(int id, CreateUserRequest request)
    {
        var user = await _context.Users.FindAsync(id);
        if (user is null) return null;

        var emailTaken = await _context.Users
            .AnyAsync(u => u.Email == request.Email && u.Id != id);
        if (emailTaken) return null;

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.Email = request.Email;
        user.PhoneNumber = request.PhoneNumber;

        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user is null) return false;

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return true;
    }
}