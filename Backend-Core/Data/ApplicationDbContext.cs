using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Face_Recognition_Demo.Models;

namespace Face_Recognition_Demo.Data
{
    public class ApplicationDbContext : IdentityDbContext<User, Role, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed Roles
            // modelBuilder.Entity<Role>().HasData(
            //     new Role { Id = "1", Name = "Admin", NormalizedName = "ADMIN", Description = "Administrator Role" },
            //     new Role { Id = "2", Name = "User", NormalizedName = "USER", Description = "Normal User Role" }
            // );

            // Seed Roles with valid GUIDs
            // modelBuilder.Entity<Role>().HasData(
            //     new Role { 
            //         Id = "11111111-1111-1111-1111-111111111111", 
            //         Name = "Admin", 
            //         NormalizedName = "ADMIN", 
            //         Description = "Administrator Role" 
            //     },
            //     new Role { 
            //         Id = "22222222-2222-2222-2222-222222222222", 
            //         Name = "User", 
            //         NormalizedName = "USER", 
            //         Description = "Normal User Role" 
            //     }
            // );
        }
    }
}