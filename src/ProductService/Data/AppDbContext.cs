using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using ProductService.Models;

namespace ProductService.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        
        modelBuilder.Entity<Product>().ToTable("Products");

    
  



        modelBuilder.Entity<Product>()
            .Property(p => p.Price)
            .HasPrecision(18, 2);

        
        modelBuilder.Entity<Product>().HasData(
            new Product { Id = -1, Name = "Laptop", Price = 1500, Stock = 10, CreatedAt = new DateTime(2026, 1, 1) },
            new Product { Id = -2, Name = "Mouse", Price = 50, Stock = 100, CreatedAt = new DateTime(2026, 1, 1) },
            new Product { Id = -3, Name = "Keyboard", Price = 75, Stock = 50, CreatedAt = new DateTime(2026, 1, 1) }

        );
    }
}