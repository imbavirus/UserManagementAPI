using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using UserProfileBackend.Application.Models.User;
using Xunit;

namespace UserProfileBackend.Tests.IntegrationTests;

public class AppDbContextTests
{
    [Fact]
    public void OnModelCreating_ShouldSeedInitialRoles()
    {
        // Arrange
        // Use the TestDbContextFactory to get an in-memory database context.
        // Creating the context via factory also calls `Database.EnsureCreated()`,
        // which triggers `OnModelCreating` and thus the `HasData` seeding.
        using var dbContextFactory = new TestDbContextFactory();
        using var dbContext = dbContextFactory.CreateContext();

        // Get the expected roles directly from the source.
        // .ToList() materializes the collection for stable comparison.
        var expectedRoles = Role.GetInitialRoles().ToList();

        // Act
        // Retrieve all roles from the database.
        // .AsNoTracking() is a good practice for read-only queries in tests,
        // as it improves performance and avoids potential side effects of change tracking.
        var actualRoles = dbContext.Roles.AsNoTracking().ToList();

        // Assert
        // 1. Ensure that roles were actually seeded.
        actualRoles.Should().NotBeNullOrEmpty(
            because: "initial roles should be seeded by OnModelCreating");

        // 2. Ensure the count of roles in the database matches the expected count.
        actualRoles.Should().HaveSameCount(expectedRoles,
            because: "the number of seeded roles should exactly match the number of initial roles defined");

        // 3. Ensure each expected role exists in the database with all properties matching.
        // For `HasData` with explicitly defined entities (including IDs and Guids),
        // we expect an exact match.
        // `CreatedOn` and `UpdatedOn` will be their default values (DateTime.MinValue)
        // if not set in `GetInitialRoles()`, and this should match the state of
        // the `expectedRole` objects as well, as `SetAuditProperties` is not
        // invoked during `HasData` seeding via `EnsureCreated()`.
        foreach (var expectedRole in expectedRoles)
        {
            actualRoles.Should().ContainEquivalentOf(expectedRole,
                because: $"role '{expectedRole.Name}' with Id '{expectedRole.Id}' should be seeded correctly with all its properties matching the definition in GetInitialRoles()");
        }
    }
    [Fact]
    public async Task SaveChanges_ShouldSetAuditPropertiesCorrectly()
    {
        // Arrange
        using var dbContextFactory = new TestDbContextFactory();
        using var dbContext = dbContextFactory.CreateContext();

        // Get a valid RoleId from the seeded roles
        var seededAdminRole = Role.GetInitialRoles().First(r => r.Name == "Admin");
        var adminRoleId = seededAdminRole.Id;

        var newUserProfile = new UserProfile("Audit Test User", "audit@example.com", adminRoleId, "Testing audit properties.")
        {
            Guid = Guid.NewGuid() // Assign a new Guid for this test entity
        };

        // Act (Add)
        var utcNowBeforeAdd = DateTime.UtcNow;
        dbContext.UserProfiles.Add(newUserProfile);
        await dbContext.SaveChangesAsync();

        // Assert (Add)
        // Detach and re-fetch to ensure we're getting data from the DB, not just the tracked entity.
        dbContext.Entry(newUserProfile).State = EntityState.Detached;
        var addedProfile = await dbContext.UserProfiles.FindAsync(newUserProfile.Id);

        addedProfile.Should().NotBeNull(because: "the user profile should have been saved to the database");
        addedProfile.CreatedOn.Should().BeCloseTo(utcNowBeforeAdd, TimeSpan.FromSeconds(2),
            because: "CreatedOn should be set to the current UTC time when an entity is added");
        addedProfile.UpdatedOn.Should().Be(addedProfile.CreatedOn,
            because: "UpdatedOn should be the same as CreatedOn for a newly added entity");

        // Arrange (Modify)
        var originalCreatedOn = addedProfile.CreatedOn;
        var originalUpdatedOn = addedProfile.UpdatedOn;

        // Ensure a small delay so UtcNow is different enough to be noticeable
        await Task.Delay(50);

        addedProfile.Name = "Updated Audit Test User";
        addedProfile.Bio = "Bio has been updated.";
        var utcNowBeforeModify = DateTime.UtcNow;

        // Act (Modify)
        // Re-attach the modified entity for saving
        dbContext.UserProfiles.Update(addedProfile);
        await dbContext.SaveChangesAsync();

        // Assert (Modify)
        dbContext.Entry(addedProfile).State = EntityState.Detached;
        var modifiedProfile = await dbContext.UserProfiles.FindAsync(addedProfile.Id);

        modifiedProfile.Should().NotBeNull();
        modifiedProfile.Name.Should().Be("Updated Audit Test User");
        modifiedProfile.Bio.Should().Be("Bio has been updated.");
        modifiedProfile.CreatedOn.Should().Be(originalCreatedOn,
            because: "CreatedOn should not change when an entity is updated");
        modifiedProfile.UpdatedOn.Should().BeCloseTo(utcNowBeforeModify, TimeSpan.FromSeconds(2),
            because: "UpdatedOn should be set to the current UTC time when an entity is modified");
        modifiedProfile.UpdatedOn.Should().BeAfter(originalUpdatedOn,
            because: "UpdatedOn should be later than its previous value after modification");
    }
}
