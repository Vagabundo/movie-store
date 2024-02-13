namespace MovieStore.Domain;
public class Order : EntityBase
{
    public Guid Branch { get; set; }
    public string Description { get; set; }
    public double Amount { get; set; }
}