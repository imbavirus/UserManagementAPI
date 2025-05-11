using UserProfileBackend.Application.Models.User;

namespace UserProfileBackend.Api.Services.User;

public interface IUserProfileService
{
    // UserProfile methods
    Task<IUserProfile> GetUserProfileByIdAsync(ulong id);
    Task<IEnumerable<IUserProfile>> GetAllUserProfilesAsync();
    Task<IUserProfile> UpdateUserProfileAsync(UserProfile userProfileUpdate);
    Task<IUserProfile> CreateUserProfileAsync(UserProfile newUserProfile);
}
