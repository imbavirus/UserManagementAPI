using Microsoft.EntityFrameworkCore;
using UserProfileBackend.Application.Models.User;

namespace UserProfileBackend.Application.Data
{

    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Role> Roles { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Create default roles
            modelBuilder.Entity<Role>().HasData(
                new Role("User")
                {
                    Id = 1UL,
                    Guid = new Guid("a1b2c3d4-e5f6-7788-99a0-bcdef1234567"),
                    CreatedOn = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                },
                new Role("Admin")
                {
                    Id = 2UL,
                    Guid = new Guid("b2c3d4e5-f6a7-8899-a0b1-cdef12345678"),
                    CreatedOn = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                },
                new Role("Moderator")
                {
                    Id = 3UL,
                    Guid = new Guid("c3d4e5f6-a7b8-99a0-b1c2-def123456789"),
                    CreatedOn = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                }
            );
        }
    }
}