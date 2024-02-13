namespace MovieStore.Api.Data;

public class RegisterMovieRequest
{
    public string Title { get; set; }
    public string Description { get; set; }
    public string Genre { get; set; }
    public string Country { get; set; }
    public int Year { get; set; }
}