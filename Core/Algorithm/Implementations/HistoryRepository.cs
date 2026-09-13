using Core.Algorithm.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Core.Algorithm.Implementations;

public class HistoryRepository(AppDbContext db) : IHistoryRepository
{
    private readonly AppDbContext _db = db;

    public async Task SaveAsync(string expression, double result)
    {
        _db.History.Add(new Calculation { Expression = expression, Result = result });
        await _db.SaveChangesAsync();
    }

    public async Task<IEnumerable<CalculationDto>> GetRecentAsync(int count = 20)
    {
        return await _db.History
            .AsNoTracking()
            .OrderByDescending(h => h.CreatedAt)
            .Take(count)
            .Select(h => new CalculationDto(h.Expression, h.Result))
            .ToListAsync();
    }
}