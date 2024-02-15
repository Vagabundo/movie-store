using System.ComponentModel.DataAnnotations.Schema;

namespace MovieStore.Domain;
public class Branch : EntityBase
{
    public Guid UserId { get; set; }
    [ForeignKey("UserId")]
    public virtual User BranchUser  { get; set; }
    public string Address { get; set; }
    public string City { get; set; }
    public string Country { get; set; }
    public string PostalCode { get; set; }
}