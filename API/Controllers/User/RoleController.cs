using Microsoft.AspNetCore.Mvc;
using UserProfileBackend.Api.Services.User;
using UserProfileBackend.Application.Models.User;

namespace UserProfileBackend.Api.Controllers.User;

[ApiController]
[Route("api/[controller]")]
public class RolesController(IRoleService roleService, ILogger<RolesController> logger) : ControllerBase
{
    private readonly IRoleService _roleService = roleService ?? throw new ArgumentNullException(nameof(roleService));
    private readonly ILogger<RolesController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <summary>
    /// Gets a specific role by its ID.
    /// </summary>
    /// <param name="id">The ID of the role to retrieve.</param>
    /// <returns>The role if found; otherwise, NotFound.</returns>
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(IRole), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IRole>> GetRoleById(ulong id)
    {
        _logger.LogInformation("Attempting to get role with ID: {RoleId}", id);
        var role = await _roleService.GetRoleByIdAsync(id);
        _logger.LogInformation("Successfully retrieved role with ID: {RoleId}", id);
        return Ok(role);
    }

    /// <summary>
    /// Gets all roles.
    /// </summary>
    /// <returns>A list of all roles.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<IRole>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<IRole>>> GetAllRoles()
    {
        _logger.LogInformation("Attempting to get all roles");
        var roles = await _roleService.GetAllRolesAsync();
        _logger.LogInformation("Successfully retrieved {count} roles", roles.Count());
        return Ok(roles);
    }

    /// <summary>
    /// Creates a new role.
    /// </summary>
    /// <param name="role">The role to create.</param>
    /// <returns>The created role.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(IRole), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IRole>> CreateRole([FromBody] Role role)
    {
        _logger.LogInformation("Attempting to create a new role with name: {RoleName}", role.Name);
        var createdRole = await _roleService.CreateRoleAsync(role);
        _logger.LogInformation("Successfully created role with ID: {RoleId}", createdRole.Id);
        return CreatedAtAction(nameof(GetRoleById), new { id = createdRole.Id }, createdRole);
    }

    /// <summary>
    /// Updates an existing role.
    /// </summary>
    /// <param name="roleUpdate">The role data to update.</param>
    /// <returns>NoContent if successful; otherwise, BadRequest or NotFound.</returns>
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateRole([FromBody] Role roleUpdate)
    {
        _logger.LogInformation("Attempting to update role with ID: {RoleId}", roleUpdate.Id);
        await _roleService.UpdateRoleAsync(roleUpdate);
        _logger.LogInformation("Successfully updated role with ID: {RoleId}", roleUpdate.Id);
        return Ok(roleUpdate);
    }
}