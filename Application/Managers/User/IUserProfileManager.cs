using UserManagementAPI.Application.Models.User;

namespace UserManagementAPI.Application.Managers.User;

public interface IUserProfileManager
{
    Task<IUserProfile> GetUserProfileByIdAsync(ulong id);
    Task<IEnumerable<IUserProfile>> GetAllUserProfilesAsync();
    Task<IUserProfile> UpdateUserProfileAsync(UserProfile userProfileUpdate);
    Task<IUserProfile> CreateUserProfileAsync(UserProfile newUserProfile);
}
