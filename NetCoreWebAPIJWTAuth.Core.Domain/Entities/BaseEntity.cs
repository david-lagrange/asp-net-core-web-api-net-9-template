using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace NetCoreWebAPIJWTAuth.Core.Domain.Entities;

public class BaseEntity
{
    [Column("BaseEntityId")]
    public Guid Id { get; set; }

    [Required(ErrorMessage = "BaseEntity name is a required field.")]
    [MaxLength(60, ErrorMessage = "Maximum length for the Name is 60 characters.")]
    public string? Name { get; set; }

    [Required(ErrorMessage = "BaseEntity address is a required field.")]
    [MaxLength(60, ErrorMessage = "Maximum length for the Address is 60 characters")]
    public string? Address { get; set; }

    public string? Country { get; set; }

    public ICollection<DependantEntity>? DependantEntities { get; set; }
}
