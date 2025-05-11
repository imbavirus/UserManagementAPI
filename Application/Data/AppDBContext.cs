using Microsoft.EntityFrameworkCore;
using UserProfileBackend.Application.Models;
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
            modelBuilder.Entity<Role>().HasData(Role.GetInitialRoles());
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            SetAuditProperties();
            return base.SaveChangesAsync(cancellationToken);
        }

        public override int SaveChanges()
        {
            SetAuditProperties();
            return base.SaveChanges();
        }

        private void SetAuditProperties()
        {
            var entries = ChangeTracker
                .Entries()
                .Where(e => e.Entity is IBaseModel && (
                        e.State == EntityState.Added ||
                        e.State == EntityState.Modified));

            foreach (var entityEntry in entries)
            {
                var dateNow = DateTime.UtcNow;
                var baseModel = (IBaseModel)entityEntry.Entity;

                if (entityEntry.State == EntityState.Added)
                {
                    baseModel.CreatedOn = dateNow;
                    baseModel.UpdatedOn = dateNow;
                }
                else if (entityEntry.State == EntityState.Modified)
                {
                    baseModel.UpdatedOn = dateNow;
                    entityEntry.Property(nameof(IBaseModel.CreatedOn)).IsModified = false;
                }
            }
        }
    }
}