using Microsoft.EntityFrameworkCore;
using UserManagementAPI.Application.Data;
using UserManagementAPI.Application.Models.User;

namespace UserManagementAPI.Application.Managers.User.Implementation;

public class RoleManager(AppDbContext context) : IRoleManager
{
    private readonly AppDbContext _context = context;

    /// <summary>
    /// Retrieves a role by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the role.</param>
    /// <returns>The <see cref="IRole"/> if found; otherwise, null.</returns>
    public async Task<IRole?> GetRoleByIdAsync(ulong id)
    {
        return await _context.Roles.FirstOrDefaultAsync(r => r.Id == id);
    }

    /// <summary>
    /// Retrieves all roles.
    /// </summary>
    /// <returns>An enumerable collection of <see cref="IRole"/>.</returns>
    public async Task<IEnumerable<IRole>> GetAllRolesAsync()
    {
        return await _context.Roles.ToListAsync();
    }

    /// <summary>
    /// Updates an existing role.
    /// </summary>
    /// <param name="roleUpdate">The role object with updated information.</param>
    /// <returns>The updated <see cref="IRole"/>.</returns>
    /// <exception cref="KeyNotFoundException">Thrown if the role with the specified Id does not exist.</exception>
    /// <exception cref="InvalidOperationException">Thrown if another role with the same name already exists.</exception>
    public async Task<IRole> UpdateRoleAsync(Role roleUpdate)
    {
        var existingRole = await _context.Roles.FindAsync(roleUpdate.Id) ??
            throw new KeyNotFoundException($"Role with Id '{roleUpdate.Id}' does not exist.");

        // Check if another role with the new name already exists (excluding the current role)
        var roleWithSameName = await _context.Roles
            .FirstOrDefaultAsync(r => r.Name == roleUpdate.Name && r.Id != roleUpdate.Id);

        if (roleWithSameName != null)
            throw new InvalidOperationException($"Another role with the name '{roleUpdate.Name}' already exists.");

        existingRole.Name = roleUpdate.Name;

        _context.Roles.Update(existingRole);
        await _context.SaveChangesAsync();

        return existingRole;
    }

    /// <summary>
    /// Creates a new role.
    /// </summary>
    /// <param name="newRole">The role object to create.</param>
    /// <returns>The created <see cref="IRole"/>.</returns>
    /// <exception cref="InvalidOperationException">Thrown if a role with the same Guid or name already exists.</exception>
    public async Task<IRole> CreateRoleAsync(Role newRole)
    {
        // Verify a role with this guid doesn't exist
        if (newRole.Guid != Guid.Empty)
        {
            var existingRoleByGuid = await _context.Roles.FirstOrDefaultAsync(r => r.Guid == newRole.Guid);

            if (existingRoleByGuid != null)
                throw new InvalidOperationException($"A Role with Guid '{newRole.Guid}' already exists.");
        }

        // Check if a role with the same name already exists
        var existingRoleByName = await _context.Roles.FirstOrDefaultAsync(r => r.Name == newRole.Name);

        if (existingRoleByName != null)
            throw new InvalidOperationException($"A Role with name '{newRole.Name}' already exists.");

        _context.Roles.Add(newRole);
        await _context.SaveChangesAsync();

        return newRole;
    }
}
