using System.ComponentModel.DataAnnotations.Schema;

namespace MovieStore.Domain;

public class BranchMovie : Trackable
{
    public Guid BranchId { get; set; }
    [ForeignKey("BranchId")]
    public virtual Branch Branch  { get; set; }

    public Guid MovieId { get; set; }
    [ForeignKey("MovieId")]
    public virtual Movie Movie  { get; set; }
}
