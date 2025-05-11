using UserProfileBackend.Application.Models.User;

namespace UserProfileBackend.Application.Managers.User;

public interface IRoleManager
{
    Task<IRole?> GetRoleByIdAsync(ulong id);
    Task<IEnumerable<IRole>> GetAllRolesAsync();
    Task<IRole> UpdateRoleAsync(Role roleUpdate);
    Task<IRole> CreateRoleAsync(Role newRole);
}
