using UserManagementAPI.Application.Models.User;

namespace UserManagementAPI.Api.Services.User;

public interface IUserProfileService
{
    // UserProfile methods
    Task<IUserProfile> GetUserProfileByIdAsync(ulong id);
    Task<IEnumerable<IUserProfile>> GetAllUserProfilesAsync();
    Task<IUserProfile> UpdateUserProfileAsync(UserProfile userProfileUpdate);
    Task<IUserProfile> CreateUserProfileAsync(UserProfile newUserProfile);
}
