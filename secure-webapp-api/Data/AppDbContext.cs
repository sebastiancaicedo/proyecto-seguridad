using Microsoft.EntityFrameworkCore;

namespace secure_webapp_api;

public class AppDbContext : DbContext
{
  public DbSet<UserCreds> UserCreds { get; set; } = null!;
  public DbSet<User> Users { get; set; } = null!;

  public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
  {

  }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<UserCreds>()
    .HasKey(x => x.Id);

    modelBuilder.Entity<UserCreds>()
    .Property(x => x.Id)
    .ValueGeneratedOnAdd();

    modelBuilder.Entity<User>()
    .HasKey(x => x.Id);

    modelBuilder.Entity<User>()
    .Property(x => x.Id)
    .ValueGeneratedOnAdd();

    base.OnModelCreating(modelBuilder);
  }
}
