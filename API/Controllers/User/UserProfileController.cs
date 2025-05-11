using Microsoft.AspNetCore.Mvc;
using UserManagementAPI.Api.Services.User;
using UserManagementAPI.Application.Models.User;

namespace UserManagementAPI.Api.Controllers.User;

[ApiController]
[Route("api/[controller]")]
public class UserProfilesController(IUserProfileService userProfileService, ILogger<UserProfilesController> logger) : ControllerBase
{
    private readonly IUserProfileService _userProfileService = userProfileService ?? throw new ArgumentNullException(nameof(userProfileService));
    private readonly ILogger<UserProfilesController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <summary>
    /// Gets a specific User Profile by its ID.
    /// </summary>
    /// <param name="id">The ID of the User Profile to retrieve.</param>
    /// <returns>The User Profile if found; otherwise, NotFound.</returns>
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(IUserProfile), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IUserProfile>> GetUserProfileById(ulong id)
    {
        _logger.LogInformation("Attempting to get userProfile with ID: {UserProfileId}", id);
        var userProfile = await _userProfileService.GetUserProfileByIdAsync(id);
        _logger.LogInformation("Successfully retrieved userProfile with ID: {id}", id);
        return Ok(userProfile);
    }

    /// <summary>
    /// Gets all User Profiles.
    /// </summary>
    /// <returns>A list of all User Profiles.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<IUserProfile>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<IUserProfile>>> GetAllUserProfiles()
    {
        _logger.LogInformation("Attempting to get all userProfiles");
        var userProfiles = await _userProfileService.GetAllUserProfilesAsync();
        _logger.LogInformation("Successfully retrieved {count} userProfiles", userProfiles.Count());
        return Ok(userProfiles);
    }

    /// <summary>
    /// Creates a new User Profile.
    /// </summary>
    /// <param name="userProfile">The User Profile to create.</param>
    /// <returns>The created User Profile.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(IUserProfile), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IUserProfile>> CreateUserProfile([FromBody] UserProfile userProfile)
    {
        _logger.LogInformation("Attempting to create a new userProfile with name: {UserProfileName}", userProfile.Name);
        var createdUserProfile = await _userProfileService.CreateUserProfileAsync(userProfile);
        _logger.LogInformation("Successfully created userProfile with ID: {UserProfileId}", createdUserProfile.Id);
        return CreatedAtAction(nameof(GetUserProfileById), new { id = createdUserProfile.Id }, createdUserProfile);
    }

    /// <summary>
    /// Updates an existing User Profile.
    /// </summary>
    /// <param name="userProfileUpdate">The User Profile data to update.</param>
    /// <returns>NoContent if successful; otherwise, BadRequest or NotFound.</returns>
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateUserProfile([FromBody] UserProfile userProfileUpdate)
    {
        _logger.LogInformation("Attempting to update userProfile with ID: {UserProfileId}", userProfileUpdate.Id);
        await _userProfileService.UpdateUserProfileAsync(userProfileUpdate);
        _logger.LogInformation("Successfully updated userProfile with ID: {UserProfileId}", userProfileUpdate.Id);
        return Ok(userProfileUpdate);
    }
}