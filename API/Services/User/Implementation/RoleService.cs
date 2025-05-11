using UserManagementAPI.Application.Managers.User;
using UserManagementAPI.Application.Models.User;

namespace UserManagementAPI.Api.Services.User.Implementation;

public class RoleService(IRoleManager roleManager) : IRoleService
{
    private readonly IRoleManager _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));

    // --- Role Methods ---

    public async Task<IRole?> GetRoleByIdAsync(ulong id)
    {
        return await _roleManager.GetRoleByIdAsync(id);
    }

    public async Task<IEnumerable<IRole>> GetAllRolesAsync()
    {
        return await _roleManager.GetAllRolesAsync();
    }

    public async Task<IRole> UpdateRoleAsync(Role roleUpdate)
    {
        return await _roleManager.UpdateRoleAsync(roleUpdate);
    }

    public async Task<IRole> CreateRoleAsync(Role newRole)
    {
        return await _roleManager.CreateRoleAsync(newRole);
    }
}
