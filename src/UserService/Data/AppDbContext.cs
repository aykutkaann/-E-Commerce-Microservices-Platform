using Microsoft.EntityFrameworkCore;
using UserService.Models;

namespace UserService.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().ToTable("Users");

        
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        // Seed data
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = -1,
                FirstName = "Aykut Kaan",
                LastName = "Altundal",
                Email = "aykut@example.com",
                PhoneNumber = "5551234567",
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                
            },
            new User
            {
                Id = -2,
                FirstName = "Jhon",
                LastName = "Dev",
                Email = "jhon@example.com",
                PhoneNumber = "5559876543",
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)

            }
        );
    }
}