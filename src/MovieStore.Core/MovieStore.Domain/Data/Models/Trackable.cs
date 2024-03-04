namespace MovieStore.Domain;

public class Trackable
{
    public DateTimeOffset Created { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTimeOffset LastModified { get; set; }
    public Guid ModifiedBy { get; set; }
    public bool IsDeleted { get; set; }
}
