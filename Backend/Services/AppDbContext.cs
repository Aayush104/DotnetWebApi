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
        public DbSet<Payment> BlogSubscriptions { get; set; }

        protected  override void OnModelCreating(ModelBuilder modelBuilder)

        {
            modelBuilder.Entity<AdBlogs>()
           .HasOne(b => b.User)
            .WithMany(u => u.Blogs)
            .HasForeignKey(b => b.UserId);

            modelBuilder.Entity<Payment>()
            .HasOne(U => U.User)
            .WithMany(bs => bs.BlogSubscriptions)
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<Payment>()
                .HasOne(bs => bs.Blog)
                .WithMany(b => b.BlogSubscriptions)
                .HasForeignKey(bs => bs.BlogId)
                 .OnDelete(DeleteBehavior.Restrict);


            base.OnModelCreating(modelBuilder);
        }

    }
}
