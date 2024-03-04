using Microsoft.EntityFrameworkCore;
using MovieStore.Domain;

namespace MovieStore.Database;

public class MovieDbContext : DbContext
{
    public MovieDbContext(DbContextOptions<MovieDbContext> options) : base(options) {}
    protected MovieDbContext(DbContextOptions options) : base(options) {}

    public DbSet<Movie> Movies { set; get; }
    public DbSet<Order> Orders { set; get; }
    public DbSet<Branch> Branches { set; get; }
    public DbSet<BranchMovie> BranchMovies { set; get; }
    public DbSet<User> AuthUsers { set; get; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        //modelBuilder.Entity<IdentityUser>().HasKey(u => u.Id);

        modelBuilder.Entity<Branch>()
                    .Property(b => b.UserId)  
                    .HasDefaultValue(Guid.Empty);

        modelBuilder.Entity<BranchMovie>()
                    .HasKey(bm => new { bm.MovieId, bm.BranchId });

        modelBuilder.Entity<BranchMovie>()
                    .HasIndex(bm => new { bm.MovieId, bm.BranchId })
                    .IsUnique()
                    .HasFilter("[IsDeleted] = 0");
    }
}