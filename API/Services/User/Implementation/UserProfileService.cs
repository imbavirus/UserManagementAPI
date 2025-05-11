using UserManagementAPI.Application.Managers.User;
using UserManagementAPI.Application.Models.User;

namespace UserManagementAPI.Api.Services.User.Implementation;

public class UserProfileService(IUserProfileManager userProfileManager, IRoleManager roleManager) : IUserProfileService
{
    private readonly IUserProfileManager _userProfileManager = userProfileManager ?? throw new ArgumentNullException(nameof(userProfileManager));
    private readonly IRoleManager _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));

    // --- UserProfile Methods ---

    public async Task<IUserProfile> GetUserProfileByIdAsync(ulong id)
    {
        return await _userProfileManager.GetUserProfileByIdAsync(id);
    }

    public async Task<IEnumerable<IUserProfile>> GetAllUserProfilesAsync()
    {
        return await _userProfileManager.GetAllUserProfilesAsync();
    }

    public async Task<IUserProfile> UpdateUserProfileAsync(UserProfile userProfileUpdate)
    {
        // Here you could add additional business logic before or after calling the manager
        // For example, sending a notification, logging, etc.
        return await _userProfileManager.UpdateUserProfileAsync(userProfileUpdate);
    }

    public async Task<IUserProfile> CreateUserProfileAsync(UserProfile newUserProfile)
    {
        // Example: Ensure the RoleId provided exists before attempting to create the user profile
        var roleExists = await _roleManager.GetRoleByIdAsync(newUserProfile.RoleId);
        if (roleExists == null)
        {
            // You might want a more specific exception type here
            throw new KeyNotFoundException($"Role with Id '{newUserProfile.RoleId}' does not exist. Cannot create user profile.");
        }
        
        var createdProfile = await _userProfileManager.CreateUserProfileAsync(newUserProfile);
        
        // Example: Post-creation logic, like sending a welcome email
        // await _emailService.SendWelcomeEmailAsync(createdProfile.Email);

        return createdProfile;
    }
}
