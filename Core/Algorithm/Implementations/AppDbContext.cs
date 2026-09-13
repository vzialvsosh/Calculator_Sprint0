using Microsoft.EntityFrameworkCore;

namespace Core.Algorithm.Implementations;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Calculation> History => Set<Calculation>();
}