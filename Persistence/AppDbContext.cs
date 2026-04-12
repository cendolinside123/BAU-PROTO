using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    private readonly string _currentTime = "CURRENT_TIMESTAMP";
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    public DbSet<Users> Users { get; set; }
    public DbSet<Products> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Users>(entity =>
        {
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql(_currentTime)
                .ValueGeneratedOnAdd();
        });
        modelBuilder.Entity<Products>(entity =>
        {
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql(_currentTime)
                .ValueGeneratedOnAdd();
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql(_currentTime)
                .ValueGeneratedOnAddOrUpdate();
        });
    }
}