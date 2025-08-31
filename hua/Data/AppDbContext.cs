using hua.Entities; // Add this line
using Microsoft.EntityFrameworkCore;

namespace hua.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Entities.Task> Tasks { get; set; }
    }
}