using Microsoft.EntityFrameworkCore;
namespace Core.Algorithm.Interfaces;

public interface IHistoryRepository
{
    Task SaveAsync(string expression, double result);
    Task<IEnumerable<CalculationDto>> GetRecentAsync(int count = 20);
}

public record CalculationDto(string Expression, double Result);