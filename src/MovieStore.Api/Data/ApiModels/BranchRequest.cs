namespace MovieStore.Api.Data;

public class RegisterBranchRequest
{
    public string Address { get; set; }
    public string City { get; set; }
    public string Country { get; set; }
    public string PostalCode { get; set; }
}

public class UpdateBranchRequest : RegisterBranchRequest
{
    public Guid Id { get; set; }
}