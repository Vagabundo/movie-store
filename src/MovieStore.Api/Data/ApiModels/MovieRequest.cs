namespace MovieStore.Api.Data;

public class RegisterMovieRequest
{
    public string Title { get; set; }
    public string Description { get; set; }
    public string Genre { get; set; }
    public string Country { get; set; }
    public int Year { get; set; }
    public double Cost { get; set; }
}

public class ModifyMovieRequest : RegisterMovieRequest
{
    public Guid Id { get; set; }
}

public class OrderRequest
{
    public Guid BranchId { get; set; }
    public List<Guid> Movies { get; set; }
    public double Amount { get; set; }
}

public class OrderResponse
{
    public string Text { get; set; }
    public OrderRequest Request { get; set; }
}