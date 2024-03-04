using System.ComponentModel.DataAnnotations.Schema;

namespace MovieStore.Domain;

public class Order : EntityBase
{
    public Guid BranchId { get; set; }
#nullable disable
    [ForeignKey("BranchId")]
    public virtual Branch Branch  { get; set; }
    public string Description { get; set; }
#nullable enable
    public double Amount { get; set; }
    public bool PaymentSucceed { get; set; } = false;
}