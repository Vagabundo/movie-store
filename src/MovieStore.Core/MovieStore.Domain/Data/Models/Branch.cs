namespace MovieStore.Domain;
public class Branch : EntityBase
{
    public string Address { get; set; }
    public string City { get; set; }
    public string Country { get; set; }
    public string PostalCode { get; set; }
}