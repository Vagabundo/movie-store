namespace MovieStore.Api.Data;

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