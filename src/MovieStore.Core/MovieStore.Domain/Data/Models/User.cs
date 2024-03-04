namespace MovieStore.Domain;

public class User : EntityBase
{
#nullable disable
    public string Name { get; set; }
    public string Email { get; set; }
#nullable enable
}