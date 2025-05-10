using System.ComponentModel.DataAnnotations;

namespace UserProfileBackend.Application.Models;

public interface IBaseModel
{
    ulong Id { get; set; }
    Guid Guid { get; set; }
    DateTime CreatedOn { get; set; }
    DateTime UpdatedOn { get; set; }

    /* These would additionally be added if I were to add user authentication */
    // ulong? CreatedByUserProfileId { get; set; }
    // UserProfile? CreatedByUserProfile { get; set; }
    // ulong? UpdatedByUserProfileId { get; set; }
    // UserProfile? UpdatedByUserProfile { get; set; }
}

public abstract class BaseModel : IBaseModel
{
    [Key]
    public ulong Id { get; set; }

    public Guid Guid { get; set; }

    public DateTime CreatedOn { get; set; }

    public DateTime UpdatedOn { get; set; }

    /* These would additionally be added if I were to add user authentication */

    // public ulong? CreatedByUserProfileId { get; set; }

    // [ForeignKey("CreatedByUserProfileId")]
    // public virtual UserProfile? CreatedByUserProfile { get; set; }

    // public ulong? UpdatedByUserProfileId { get; set; }

    // [ForeignKey("UpdatedByUserProfileId")]
    // public virtual UserProfile? UpdatedByUserProfile { get; set; }
}
