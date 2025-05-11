using UserProfileBackend.Application.Models.User;

namespace UserProfileBackend.Api.Services.User;

public interface IRoleService
{
    // Role methods
    Task<IRole?> GetRoleByIdAsync(ulong id);
    Task<IEnumerable<IRole>> GetAllRolesAsync();
    Task<IRole> UpdateRoleAsync(Role roleUpdate);
    Task<IRole> CreateRoleAsync(Role newRole);
}
