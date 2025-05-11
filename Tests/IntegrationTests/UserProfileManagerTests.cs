using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using UserProfileBackend.Application.Data;
using UserProfileBackend.Application.Managers.User.Implementation;
using UserProfileBackend.Application.Models.User;
using Xunit;

namespace UserProfileBackend.Tests.IntegrationTests;

public class UserProfileManagerTests : IDisposable
{
    private readonly TestDbContextFactory _dbContextFactory;
    private readonly AppDbContext _context; // Single context per test, managed by xUnit constructor/Dispose
    private readonly UserProfileManager _manager;
    private readonly List<Role> _initialRoles;

    public UserProfileManagerTests()
    {
        _dbContextFactory = new TestDbContextFactory();
        _context = _dbContextFactory.CreateContext(); // Seeds initial roles
        _manager = new UserProfileManager(_context);
        _initialRoles = Role.GetInitialRoles(); // Get a reference to the seeded roles
    }

    private async Task SeedUserProfilesAsync(List<UserProfile> profiles)
    {
        _context.UserProfiles.AddRange(profiles);
        await _context.SaveChangesAsync();
    }

    [Fact]
    public async Task GetUserProfileByIdAsync_ShouldReturnProfileWithRole_WhenProfileExists()
    {
        // Arrange
        var userRole = _initialRoles.First(r => r.Name == "User");
        var profileToSeed = new UserProfile("Test User", "test@example.com", userRole.Id, "Bio")
        {
            Id = 101UL, // Assign a unique ID for this test
            Guid = Guid.NewGuid()
        };
        await SeedUserProfilesAsync([profileToSeed]);

        // Act
        var result = await _manager.GetUserProfileByIdAsync(profileToSeed.Id);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(profileToSeed.Id);
        result.Name.Should().Be("Test User");
        result.Email.Should().Be("test@example.com");
        result.Role.Should().NotBeNull();
        result.Role.Id.Should().Be(userRole.Id);
        result.Role.Name.Should().Be(userRole.Name);
    }

    [Fact]
    public async Task GetUserProfileByIdAsync_ShouldThrowKeyNotFoundException_WhenProfileDoesNotExist()
    {
        // Arrange
        var nonExistentId = 999UL;

        // Act
        Func<Task> act = async () => await _manager.GetUserProfileByIdAsync(nonExistentId);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"User Profile with Id '{nonExistentId}' does not exist.");
    }

    [Fact]
    public async Task GetAllUserProfilesAsync_ShouldReturnAllProfilesWithRoles_WhenProfilesExist()
    {
        // Arrange
        var adminRole = _initialRoles.First(r => r.Name == "Admin");
        var userRole = _initialRoles.First(r => r.Name == "User");
        var profilesToSeed = new List<UserProfile>
        {
            new("Alice", "alice@example.com", adminRole.Id) { Id = 201UL, Guid = Guid.NewGuid() },
            new("Bob", "bob@example.com", userRole.Id) { Id = 202UL, Guid = Guid.NewGuid() }
        };
        await SeedUserProfilesAsync(profilesToSeed);

        // Act
        var results = (await _manager.GetAllUserProfilesAsync()).ToList();

        // Assert
        results.Should().HaveCount(2);
        results.Should().Contain(up => up.Name == "Alice" && up.Role != null && up.Role.Name == "Admin");
        results.Should().Contain(up => up.Name == "Bob" && up.Role != null && up.Role.Name == "User");
    }

    [Fact]
    public async Task GetAllUserProfilesAsync_ShouldReturnEmptyList_WhenNoProfilesExist()
    {
        // Arrange: No profiles seeded beyond what the constructor does (which is none for UserProfiles)

        // Act
        var results = await _manager.GetAllUserProfilesAsync();

        // Assert
        results.Should().BeEmpty();
    }

    [Fact]
    public async Task CreateUserProfileAsync_ShouldCreateAndReturnProfile_WhenDataIsValid()
    {
        // Arrange
        var userRole = _initialRoles.First(r => r.Name == "User");
        var newUserProfile = new UserProfile("New User", "newuser@example.com", userRole.Id, "New Bio")
        {
            Id = 301UL, // Explicitly set ID for creation, as per manager's check
            Guid = Guid.NewGuid()
        };
        var utcNowBeforeCreate = DateTime.UtcNow;

        // Act
        var createdProfile = await _manager.CreateUserProfileAsync(newUserProfile);

        // Assert
        createdProfile.Should().NotBeNull();
        createdProfile.Id.Should().Be(newUserProfile.Id);
        createdProfile.Name.Should().Be("New User");
        createdProfile.Email.Should().Be("newuser@example.com");
        createdProfile.RoleId.Should().Be(userRole.Id);
        createdProfile.Role.Should().BeNull(because: "CreateUserProfileAsync doesn't explicitly load navigation properties on the returned object");

        // Verify from DB
        var savedProfile = await _context.UserProfiles.Include(up => up.Role).FirstOrDefaultAsync(up => up.Id == createdProfile.Id);
        savedProfile.Should().NotBeNull();
        savedProfile.Name.Should().Be("New User");
        savedProfile.CreatedOn.Should().BeCloseTo(utcNowBeforeCreate, TimeSpan.FromSeconds(2));
        savedProfile.UpdatedOn.Should().BeCloseTo(savedProfile.CreatedOn, TimeSpan.FromMilliseconds(50)); // Allow small diff due to DB precision
        savedProfile.Role.Should().NotBeNull();
        savedProfile.Role.Name.Should().Be("User");
    }

    [Fact]
    public async Task CreateUserProfileAsync_ShouldThrowInvalidDataException_WhenIdExists()
    {
        // Arrange
        var userRole = _initialRoles.First(r => r.Name == "User");
        var existingProfile = new UserProfile("Existing User", "existing@example.com", userRole.Id)
        {
            Id = 302UL, Guid = Guid.NewGuid()
        };
        await SeedUserProfilesAsync([existingProfile]);

        var newUserProfileWithSameId = new UserProfile("Another User", "another@example.com", userRole.Id)
        {
            Id = existingProfile.Id, // Same ID
            Guid = Guid.NewGuid()    // Different Guid
        };

        // Act
        Func<Task> act = async () => await _manager.CreateUserProfileAsync(newUserProfileWithSameId);

        // Assert
        await act.Should().ThrowAsync<InvalidDataException>()
            .WithMessage("UserProfile with this Id or Guid already exists.");
    }

    [Fact]
    public async Task CreateUserProfileAsync_ShouldThrowInvalidDataException_WhenGuidExists()
    {
        // Arrange
        var userRole = _initialRoles.First(r => r.Name == "User");
        var existingProfile = new UserProfile("Existing User", "existing@example.com", userRole.Id)
        {
            Id = 303UL, Guid = Guid.NewGuid()
        };
        await SeedUserProfilesAsync([existingProfile]);

        var newUserProfileWithSameGuid = new UserProfile("Yet Another User", "yetanother@example.com", userRole.Id)
        {
            Id = 304UL,                 // Different ID
            Guid = existingProfile.Guid // Same Guid
        };

        // Act
        Func<Task> act = async () => await _manager.CreateUserProfileAsync(newUserProfileWithSameGuid);

        // Assert
        await act.Should().ThrowAsync<InvalidDataException>()
            .WithMessage("UserProfile with this Id or Guid already exists.");
    }


    [Fact]
    public async Task UpdateUserProfileAsync_ShouldUpdateAndReturnProfile_WhenDataIsValid()
    {
        // Arrange
        var userRole = _initialRoles.First(r => r.Name == "User");
        var adminRole = _initialRoles.First(r => r.Name == "Admin");
        var profileToUpdate = new UserProfile("Old Name", "oldemail@example.com", userRole.Id, "Old Bio")
        {
            Id = 401UL, Guid = Guid.NewGuid()
        };
        await SeedUserProfilesAsync([profileToUpdate]);
        var originalCreatedOn = (await _context.UserProfiles.AsNoTracking().FirstAsync(up => up.Id == profileToUpdate.Id)).CreatedOn;

        var updateDto = new UserProfile("New Name", "newemail@example.com", adminRole.Id, "New Bio")
        {
            Id = profileToUpdate.Id, // Must match
            Guid = profileToUpdate.Guid, // Guid typically not changed by this DTO
            ReceiveNewsletter = true
        };
        await Task.Delay(50); // Ensure UpdatedOn will be different
        var utcNowBeforeUpdate = DateTime.UtcNow;

        // Act
        var updatedProfile = await _manager.UpdateUserProfileAsync(updateDto);

        // Assert - on returned object
        updatedProfile.Should().NotBeNull();
        updatedProfile.Id.Should().Be(profileToUpdate.Id);
        updatedProfile.Name.Should().Be("New Name");
        updatedProfile.Email.Should().Be("newemail@example.com");
        updatedProfile.Bio.Should().Be("New Bio");
        updatedProfile.RoleId.Should().Be(adminRole.Id);
        updatedProfile.ReceiveNewsletter.Should().BeTrue();
        // The 'Role' navigation property on the *returned* object from UpdateUserProfileAsync
        // (which is the tracked existingUserProfile) will be null because FindAsync doesn't load it.
        updatedProfile.Role.Should().BeNull();

        // Assert - from DB
        var savedProfile = await _context.UserProfiles.Include(up => up.Role).FirstOrDefaultAsync(up => up.Id == profileToUpdate.Id);
        savedProfile.Should().NotBeNull();
        savedProfile.Name.Should().Be("New Name");
        savedProfile.Email.Should().Be("newemail@example.com");
        savedProfile.Bio.Should().Be("New Bio");
        savedProfile.RoleId.Should().Be(adminRole.Id);
        savedProfile.ReceiveNewsletter.Should().BeTrue();
        savedProfile.CreatedOn.Should().Be(originalCreatedOn, because: "CreatedOn should not change on update");
        savedProfile.UpdatedOn.Should().BeCloseTo(utcNowBeforeUpdate, TimeSpan.FromSeconds(2));
        savedProfile.UpdatedOn.Should().BeAfter(originalCreatedOn);
        savedProfile.Role.Should().NotBeNull();
        savedProfile.Role.Name.Should().Be("Admin");
    }

    [Fact]
    public async Task UpdateUserProfileAsync_ShouldThrowKeyNotFoundException_WhenProfileDoesNotExist()
    {
        // Arrange
        var nonExistentProfileUpdate = new UserProfile("Non Existent", "no@example.com", _initialRoles.First().Id)
        {
            Id = 999UL, Guid = Guid.NewGuid()
        };

        // Act
        Func<Task> act = async () => await _manager.UpdateUserProfileAsync(nonExistentProfileUpdate);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"User Profile with Id '{nonExistentProfileUpdate.Id}' does not exist.");
    }

    [Fact]
    public async Task UpdateUserProfileAsync_ShouldThrowInvalidOperationException_WhenEmailExistsForAnotherProfile()
    {
        // Arrange
        var userRole = _initialRoles.First(r => r.Name == "User");
        var profile1 = new UserProfile("User One", "user1@example.com", userRole.Id)
        {
            Id = 501UL, Guid = Guid.NewGuid()
        };
        var profile2 = new UserProfile("User Two", "user2@example.com", userRole.Id)
        {
            Id = 502UL, Guid = Guid.NewGuid()
        };
        await SeedUserProfilesAsync([profile1, profile2]);

        var updateDtoForProfile1 = new UserProfile(profile1.Name, profile2.Email, profile1.RoleId) // Attempt to use profile2's email
        {
            Id = profile1.Id,
            Guid = profile1.Guid
        };

        // Act
        Func<Task> act = async () => await _manager.UpdateUserProfileAsync(updateDtoForProfile1);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage($"Another user profile with the email '{profile2.Email}' already exists.");
    }

    public void Dispose()
    {
        _context?.Dispose();
        _dbContextFactory?.Dispose();
        GC.SuppressFinalize(this);
    }
}