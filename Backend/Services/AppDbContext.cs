using Backend.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services
{
    public class AppDbContext : IdentityDbContext<Registration>
    {
        public AppDbContext(DbContextOptions options) : base (options)
        {
            
        }

        public new DbSet<Registration> Users { get; set; }
    }
}
