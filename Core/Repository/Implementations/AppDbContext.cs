using Microsoft.EntityFrameworkCore;
using Core.Repository.Interfaces;

namespace Core.Repository.Implementations;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Calculation> History => Set<Calculation>();
}