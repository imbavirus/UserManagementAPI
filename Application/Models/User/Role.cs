using System.ComponentModel.DataAnnotations;

namespace UserManagementAPI.Application.Models.User;

public interface IRole : IBaseModel
{
    string Name { get; set; }
}

public class Role(string name) : BaseModel, IRole
{

    [Required(ErrorMessage = "Role name is required.")]
    public string Name { get; set; } = name;

    public static List<Role> GetInitialRoles()
    {
        return
        [
            new("User")
            {
                Id = 1UL,
                Guid = new Guid("a1b2c3d4-e5f6-7788-99a0-bcdef1234567"),
                CreatedOn = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                UpdatedOn = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            },
            new("Admin")
            {
                Id = 2UL,
                Guid = new Guid("b2c3d4e5-f6a7-8899-a0b1-cdef12345678"),
                CreatedOn = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                UpdatedOn = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            },
            new("Moderator")
            {
                Id = 3UL,
                Guid = new Guid("c3d4e5f6-a7b8-99a0-b1c2-def123456789"),
                CreatedOn = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                UpdatedOn = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            }
        ];
    }
}