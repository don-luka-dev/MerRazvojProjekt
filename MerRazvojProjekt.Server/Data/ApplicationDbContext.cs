using Microsoft.EntityFrameworkCore;
using MerRazvojProjekt.Server.Models;

namespace MerRazvojProjekt.Server.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        public DbSet<Customer> Customers { get; set; }
        public DbSet<RequestLog> RequestLogs { get; set; }
    }
}

