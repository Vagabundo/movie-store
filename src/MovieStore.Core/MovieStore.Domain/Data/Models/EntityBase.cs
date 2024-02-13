using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieStore.Domain;
public class EntityBase
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    public DateTimeOffset Created { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTimeOffset LastModified { get; set; }
    public Guid ModifiedBy { get; set; }
    public bool IsDeleted { get; set; }
}