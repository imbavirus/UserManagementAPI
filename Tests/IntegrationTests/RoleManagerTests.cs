using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using UserManagementAPI.Application.Data;
using UserManagementAPI.Application.Managers.User.Implementation;
using UserManagementAPI.Application.Models.User;
using Xunit;

namespace UserManagementAPI.Tests.IntegrationTests;

public class RoleManagerTests
{
    private TestDbContextFactory _dbContextFactory;

    public RoleManagerTests()
    {
        _dbContextFactory = new TestDbContextFactory();
    }

    private static RoleManager CreateManager(AppDbContext context)
    {
        return new RoleManager(context);
    }

    [Fact]
    public async Task GetRoleByIdAsync_ShouldReturnRole_WhenRoleExists()
    {
        // Arrange
        using var context = _dbContextFactory.CreateContext(); // This seeds initial roles
        var manager = CreateManager(context);
        var expectedRole = Role.GetInitialRoles().First(); // Get one of the seeded roles

        // Act
        var actualRole = await manager.GetRoleByIdAsync(expectedRole.Id);

        // Assert
        actualRole.Should().NotBeNull();
        actualRole.Should().BeEquivalentTo(expectedRole, options => options
            .Excluding(r => r.CreatedOn) // HasData sets these, but manager doesn't modify them on read
            .Excluding(r => r.UpdatedOn)); // HasData sets these, but manager doesn't modify them on read

        // Explicitly check audit properties if they are part of your GetInitialRoles definition
        // and you expect them to be preserved as seeded.
        actualRole.CreatedOn.Should().Be(expectedRole.CreatedOn);
        actualRole.UpdatedOn.Should().Be(expectedRole.UpdatedOn);
    }

    [Fact]
    public async Task GetRoleByIdAsync_ShouldReturnNull_WhenRoleDoesNotExist()
    {
        // Arrange
        using var context = _dbContextFactory.CreateContext();
        var manager = CreateManager(context);
        var nonExistentId = 999UL;

        // Act
        var result = await manager.GetRoleByIdAsync(nonExistentId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllRolesAsync_ShouldReturnAllRoles_WhenRolesExist()
    {
        // Arrange
        using var context = _dbContextFactory.CreateContext(); // Seeds initial roles
        var manager = CreateManager(context);
        var expectedRoles = Role.GetInitialRoles().OrderBy(r => r.Id).ToList();

        // Act
        var actualRoles = (await manager.GetAllRolesAsync()).OrderBy(r => r.Id).ToList();

        // Assert
        actualRoles.Should().NotBeNullOrEmpty();
        actualRoles.Should().HaveSameCount(expectedRoles);
        actualRoles.Should().BeEquivalentTo(expectedRoles, options => options
            .Excluding(r => r.CreatedOn)
            .Excluding(r => r.UpdatedOn)
            .WithStrictOrdering());

        for(int i = 0; i < expectedRoles.Count; i++)
        {
            actualRoles[i].CreatedOn.Should().Be(expectedRoles[i].CreatedOn);
            actualRoles[i].UpdatedOn.Should().Be(expectedRoles[i].UpdatedOn);
        }
    }

    [Fact]
    public async Task GetAllRolesAsync_ShouldReturnEmptyList_WhenNoRolesExist()
    {
        // Arrange
        // For this test, we need a context without the initial seeded roles.
        // We can achieve this by creating a context and immediately deleting all roles.
        using var context = _dbContextFactory.CreateContext();
        context.Roles.RemoveRange(context.Roles); // Remove seeded roles
        await context.SaveChangesAsync();

        var manager = CreateManager(context);

        // Act
        var result = await manager.GetAllRolesAsync();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task CreateRoleAsync_ShouldCreateAndReturnRole_WhenDataIsValid()
    {
        // Arrange
        using var context = _dbContextFactory.CreateContext();
        var manager = CreateManager(context);
        var newRole = new Role("Test Role") { Guid = Guid.NewGuid() };
        var utcNowBeforeCreate = DateTime.UtcNow;

        // Act
        var createdRole = await manager.CreateRoleAsync(newRole);
        var savedRole = await context.Roles.FindAsync(createdRole.Id);

        // Assert
        createdRole.Should().NotBeNull();
        createdRole.Name.Should().Be("Test Role");
        createdRole.Guid.Should().Be(newRole.Guid);

        savedRole.Should().NotBeNull();
        savedRole.Name.Should().Be("Test Role");
        savedRole.Guid.Should().Be(newRole.Guid);
        savedRole.CreatedOn.Should().BeCloseTo(utcNowBeforeCreate, TimeSpan.FromSeconds(2));
        savedRole.UpdatedOn.Should().Be(savedRole.CreatedOn);
    }

    [Fact]
    public async Task CreateRoleAsync_ShouldThrowInvalidOperationException_WhenRoleWithSameNameExists()
    {
        // Arrange
        using var context = _dbContextFactory.CreateContext(); // Seeds initial roles
        var manager = CreateManager(context);
        var existingRoleName = Role.GetInitialRoles().First().Name;
        var newRole = new Role(existingRoleName) { Guid = Guid.NewGuid() };

        // Act
        Func<Task> act = async () => await manager.CreateRoleAsync(newRole);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage($"A Role with name '{existingRoleName}' already exists.");
    }

    [Fact]
    public async Task CreateRoleAsync_ShouldThrowInvalidOperationException_WhenRoleWithSameGuidExists()
    {
        // Arrange
        using var context = _dbContextFactory.CreateContext(); // Seeds initial roles
        var manager = CreateManager(context);
        var existingRoleGuid = Role.GetInitialRoles().First().Guid;
        var newRole = new Role("Unique Name For Guid Test") { Guid = existingRoleGuid };

        // Act
        Func<Task> act = async () => await manager.CreateRoleAsync(newRole);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage($"A Role with Guid '{existingRoleGuid}' already exists.");
    }
    
    [Fact]
    public async Task CreateRoleAsync_ShouldCreateRole_WhenGuidIsEmptyAndNameIsUnique()
    {
        // Arrange
        using var context = _dbContextFactory.CreateContext();
        var manager = CreateManager(context);
        var newRole = new Role("RoleWithEmptyGuid") { Guid = Guid.Empty }; // Guid.Empty should be allowed if not checked for uniqueness
        var utcNowBeforeCreate = DateTime.UtcNow;

        // Act
        var createdRole = await manager.CreateRoleAsync(newRole);
        var savedRole = await context.Roles.FindAsync(createdRole.Id);

        // Assert
        createdRole.Should().NotBeNull();
        savedRole.Should().NotBeNull();
        savedRole.Name.Should().Be("RoleWithEmptyGuid");
        savedRole.Guid.Should().Be(Guid.Empty); // Or whatever EF Core defaults it to if not set
        savedRole.CreatedOn.Should().BeCloseTo(utcNowBeforeCreate, TimeSpan.FromSeconds(2));
        savedRole.UpdatedOn.Should().Be(savedRole.CreatedOn);
    }


    [Fact]
    public async Task UpdateRoleAsync_ShouldUpdateAndReturnRole_WhenDataIsValid()
    {
        // Arrange
        using var context = _dbContextFactory.CreateContext();
        var manager = CreateManager(context);
        var roleToUpdate = Role.GetInitialRoles().First(); // Get a seeded role
        
        // Detach to simulate fetching and then updating
        context.Entry(roleToUpdate).State = EntityState.Detached; 

        var updatedRoleData = new Role("Updated Role Name")
        {
            Id = roleToUpdate.Id, // Must match the ID of the role to update
            Guid = roleToUpdate.Guid, // Guid typically doesn't change on update
            CreatedOn = roleToUpdate.CreatedOn // Preserve original CreatedOn
        };
        var utcNowBeforeUpdate = DateTime.UtcNow;
        await Task.Delay(50); // Ensure UpdatedOn will be different

        // Act
        var result = await manager.UpdateRoleAsync(updatedRoleData);
        var savedRole = await context.Roles.FindAsync(roleToUpdate.Id);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Updated Role Name");
        result.Id.Should().Be(roleToUpdate.Id);

        savedRole.Should().NotBeNull();
        savedRole.Name.Should().Be("Updated Role Name");
        savedRole.CreatedOn.Should().Be(roleToUpdate.CreatedOn); // CreatedOn should not change
        savedRole.UpdatedOn.Should().BeCloseTo(utcNowBeforeUpdate, TimeSpan.FromSeconds(2));
        savedRole.UpdatedOn.Should().BeAfter(roleToUpdate.UpdatedOn);
    }

    [Fact]
    public async Task UpdateRoleAsync_ShouldThrowKeyNotFoundException_WhenRoleDoesNotExist()
    {
        // Arrange
        using var context = _dbContextFactory.CreateContext();
        var manager = CreateManager(context);
        var nonExistentRole = new Role("Non Existent") { Id = 999UL, Guid = Guid.NewGuid() };

        // Act
        Func<Task> act = async () => await manager.UpdateRoleAsync(nonExistentRole);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"Role with Id '{nonExistentRole.Id}' does not exist.");
    }

    [Fact]
    public async Task UpdateRoleAsync_ShouldThrowInvalidOperationException_WhenUpdatingToExistingRoleName()
    {
        // Arrange
        using var context = _dbContextFactory.CreateContext(); // Seeds initial roles
        var manager = CreateManager(context);
        var roles = Role.GetInitialRoles().Take(2).ToList();
        var roleToUpdate = roles[0];
        var otherRoleName = roles[1].Name;

        var updateDto = new Role(otherRoleName) // Attempt to set name to otherRoleName
        {
            Id = roleToUpdate.Id,
            Guid = roleToUpdate.Guid
        };

        // Act
        Func<Task> act = async () => await manager.UpdateRoleAsync(updateDto);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage($"Another role with the name '{otherRoleName}' already exists.");
    }
}
