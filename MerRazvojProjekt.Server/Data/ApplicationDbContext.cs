using Microsoft.EntityFrameworkCore;
using MerRazvojProjekt.Server.Models;

namespace MerRazvojProjekt.Server.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Car>()
                .Property(c => c.Price)
                .HasPrecision(18, 2);
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<RequestLog> RequestLogs { get; set; }
        public DbSet<Car> Cars { get; set; }
    }
}

