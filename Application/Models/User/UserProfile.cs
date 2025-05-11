using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace UserManagementAPI.Application.Models.User;
public interface IUserProfile : IBaseModel
{
    string Name { get; set; }
    string Email { get; set; }
    string? Bio { get; set; }
    ulong RoleId { get; set; }
    Role? Role { get; set; }
    bool ReceiveNewsletter { get; set; }
}

public class UserProfile(string name, string email, ulong roleId, string? bio = null, bool receiveNewsletter = false) : BaseModel,  IUserProfile
{
    [Required(ErrorMessage = "Name is required.")]
    public string Name { get; set; } = name;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
    public string Email { get; set; } = email;

    [StringLength(500, ErrorMessage = "Bio cannot be longer than 500 characters.")]
    public string? Bio { get; set; } = bio;

    [Required(ErrorMessage = "Role is required.")]
    public ulong RoleId { get; set; } = roleId;
    [ForeignKey("RoleId")]
    public Role? Role { get; set; }
    public bool ReceiveNewsletter { get; set; } = receiveNewsletter;
}
