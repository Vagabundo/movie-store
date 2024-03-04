namespace MovieStore.Domain;

public class Movie : EntityBase
{
#nullable disable
    public string Title { get; set; }
    public string Description { get; set; }
    public string Genre { get; set; }
    public string Country { get; set; }
#nullable enable
    public int Year { get; set; }
    public double Cost { get; set; }
}