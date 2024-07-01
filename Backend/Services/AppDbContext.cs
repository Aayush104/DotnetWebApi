using Backend.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;

namespace Backend.Services
{
    public class AppDbContext : IdentityDbContext<Registration>
    {
        internal object AdBlogs;

        public AppDbContext(DbContextOptions options) : base (options)
        {
            
        }

        public new DbSet<Registration> Users { get; set; }
        public DbSet<AdBlogs> Blog { get; set; }

        protected void OnModelCreating(ModelBuilder modelBuilder)

        {
            modelBuilder.Entity<AdBlogs>()
           .HasOne(b => b.User)
            .WithMany(u => u.Blogs)
            .HasForeignKey(b => b.UserId);

            base.OnModelCreating(modelBuilder);
        }

    }
}
