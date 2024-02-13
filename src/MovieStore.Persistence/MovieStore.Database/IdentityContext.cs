using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MovieStore.Database;

public class IdentityContext : IdentityDbContext<IdentityUser<Guid>, IdentityRole<Guid>, Guid>
{
    public IdentityContext(DbContextOptions<IdentityContext> options) : base(options) {}
    protected IdentityContext(DbContextOptions options) : base(options) {}

    // protected override void OnModelCreating(ModelBuilder modelBuilder)
    // {
    //     base.OnModelCreating(modelBuilder);
    //     modelBuilder.Entity<UserProfile>().Property(b => b.IsDeleted).HasDefaultValue(false);
    //     //modelBuilder.Entity<IdentityUser>().HasKey(u => u.Id);

    //     modelBuilder.Entity<DeskTask>().Property(b => b.IsDeleted).HasDefaultValue(false);

    //     modelBuilder.Entity<Notification>().Property(b => b.IsDeleted).HasDefaultValue(false);
    // }
}