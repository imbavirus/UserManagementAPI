using System.ComponentModel.DataAnnotations;

namespace UserProfileBackend.Application.Models.User;

public interface IRole : IBaseModel
{
    string Name { get; set; }
}

public class Role(string name) : BaseModel, IRole
{

    [Required(ErrorMessage = "Role name is required.")]
    public string Name { get; set; } = name;
}