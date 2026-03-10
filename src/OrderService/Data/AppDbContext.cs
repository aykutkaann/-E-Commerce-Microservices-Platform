using Microsoft.EntityFrameworkCore;
using OrderService.Models;
namespace OrderService.Data
{
    public class AppDbContext : DbContext
    {

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Order> Orders => Set<Order>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Order>().ToTable("Orders");

            modelBuilder.Entity<Order>()
                .Property(o => o.TotalPrice)
                .HasPrecision(18, 2);

            base.OnModelCreating(modelBuilder);
        }

    }
}
