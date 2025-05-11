using Microsoft.EntityFrameworkCore;
using UserProfileBackend.Application.Data;
using UserProfileBackend.Application.Models.User;

namespace UserProfileBackend.Application.Managers.User.Implementation;

public class UserProfileManager(AppDbContext context) : IUserProfileManager
{
    private readonly AppDbContext _context = context;

    /// <summary>
    /// Retrieves a user profile by its unique identifier, including its associated role.
    /// </summary>
    /// <param name="id">The unique identifier of the user profile.</param>
    /// <returns>The <see cref="IUserProfile"/> if found; otherwise, throws a <see cref="KeyNotFoundException"/>.</returns>
    /// <exception cref="KeyNotFoundException">Thrown if the user profile with the specified Id does not exist.</exception>
    public async Task<IUserProfile> GetUserProfileByIdAsync(ulong id)
    {
        return await _context.UserProfiles
            .Include(x => x.Role)
            .FirstOrDefaultAsync(x => x.Id == id) ??
            throw new KeyNotFoundException($"User Profile with Id '{id}' does not exist.");
    }

    /// <summary>
    /// Retrieves all user profiles, including their associated roles.
    /// </summary>
    /// <returns>An enumerable collection of <see cref="IUserProfile"/>.</returns>
    public async Task<IEnumerable<IUserProfile>> GetAllUserProfilesAsync()
    {
        return await _context.UserProfiles
            .Include(x => x.Role)
            .ToListAsync();
    }

    /// <summary>
    /// Updates an existing user profile.
    /// </summary>
    /// <param name="userProfileUpdate">The user profile object with updated information.</param>
    /// <returns>The updated <see cref="IUserProfile"/>.</returns>
    /// <exception cref="KeyNotFoundException">Thrown if the user profile with the specified Id does not exist.</exception>
    /// <exception cref="InvalidOperationException">Thrown if another user profile with the same email already exists (when the existing logic for this check is met).</exception>
    public async Task<IUserProfile> UpdateUserProfileAsync(UserProfile userProfileUpdate)
    {
        // Check if the User Profile exists
        var existingUserProfile = await _context.UserProfiles.FindAsync(userProfileUpdate.Id) ??
            throw new KeyNotFoundException($"User Profile with Id '{userProfileUpdate.Id}' does not exist.");


        // Check if another User Profile with the same email already exists (excluding the current User Profile)
        var userProfileWithSameEmail = await _context.UserProfiles
            .FirstOrDefaultAsync(x => x.Email == userProfileUpdate.Email && x.Id != userProfileUpdate.Id);

        if (userProfileWithSameEmail != null)
            throw new InvalidOperationException($"Another user profile with the email '{userProfileUpdate.Email}' already exists.");


        existingUserProfile.Name = userProfileUpdate.Name;
        existingUserProfile.Email = userProfileUpdate.Email;
        existingUserProfile.Bio = userProfileUpdate.Bio;
        existingUserProfile.RoleId = userProfileUpdate.RoleId;
        existingUserProfile.ReceiveNewsletter = userProfileUpdate.ReceiveNewsletter;
        
        _context.UserProfiles.Update(existingUserProfile);
        await _context.SaveChangesAsync();

        return existingUserProfile;
    }

    /// <summary>
    /// Creates a new user profile.
    /// </summary>
    /// <param name="newUserProfile">The user profile object to create.</param>
    /// <returns>The created <see cref="IUserProfile"/>.</returns>
    /// <exception cref="InvalidDataException">Thrown if a user profile with the same Id or Guid already exists (when the existing logic for this check is met).</exception>
    public async Task<IUserProfile> CreateUserProfileAsync(UserProfile newUserProfile)
    {
        // Verify a user with this id or guid doesnt exist
        var existingUserProfile = await _context.UserProfiles.FirstOrDefaultAsync(x => x.Id == newUserProfile.Id || x.Guid == newUserProfile.Guid);

        if (existingUserProfile != null)
            throw new InvalidDataException("UserProfile with this Id or Guid already exists.");

        _context.UserProfiles.Add(newUserProfile);
        await _context.SaveChangesAsync();

        return newUserProfile;
    }
}
