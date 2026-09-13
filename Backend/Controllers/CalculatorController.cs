using Microsoft.AspNetCore.Mvc;
using Core.Algorithm.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers;

[ApiController]
[Route("api/calculator")]
public class CalculatorController : ControllerBase
{
    private readonly ICalculator _calculator;
    private readonly IHistoryRepository _history;

    public CalculatorController(ICalculator calculator, IHistoryRepository history)
    {
        _calculator = calculator;
        _history = history;
    }

    [HttpPost("calculate")]
    public async Task<IActionResult> Calculate([FromBody] string expression)
    {
        try
        {
            double result = _calculator.Calculate(expression);
            await _history.SaveAsync(expression, result);
            return Ok(new { result });
        }
        catch (DbUpdateException ex)
        {
            // Настоящая ошибка SQLite от СУБД:
            Console.WriteLine($"Ошибка БД: {ex.InnerException?.Message}");
            throw;
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("history")]
    public async Task<IActionResult> GetHistory([FromQuery] int count = 20)
    {
        var history = await _history.GetRecentAsync(count);
        return Ok(history);
    }
}