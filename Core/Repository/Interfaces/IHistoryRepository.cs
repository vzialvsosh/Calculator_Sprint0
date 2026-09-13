using Microsoft.EntityFrameworkCore;

namespace Core.Repository.Interfaces;

public interface IHistoryRepository
{
    Task SaveAsync(string expression, double result);
    Task<IEnumerable<CalculationDto>> GetRecentAsync(int count = 20);
    Task<bool> DeleteByIdAsync(Guid id);
    Task ClearAllAsync();
}

public record CalculationDto(string Expression, double Result, Guid Id);