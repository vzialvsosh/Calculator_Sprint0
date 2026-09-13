using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Core.Repository.Interfaces;
using Core.Algorithm.Interfaces;

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
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception)
        {
            return StatusCode(500, "An error occurred while processing the request");
        }
    }

    [HttpGet("history")]
    public async Task<IActionResult> GetHistory([FromQuery] int count = 20)
    {
        if (count <= 0)
        {
            return BadRequest("Count must be a positive integer");
        }
        try
        {
            var history = await _history.GetRecentAsync(count);
            return Ok(history);
        }
        catch (Exception)
        {
            return StatusCode(500, "An error occurred while processing the request");
        }
    }

    [HttpDelete("history/{id:guid}")]
    public async Task<IActionResult> DeleteItem(Guid id)
    {
        try {
            var deleted = await _history.DeleteByIdAsync(id);
            if (!deleted)
            {
                return NotFound($"ID {id} not founded");
            }
        }
        catch (Exception)
        {
            return StatusCode(500, "An error occurred while processing the request");
        }
        return Ok($"ID {id} deleted");
    }

    [HttpDelete("history")]
    public async Task<IActionResult> ClearHistory()
    {
        await _history.ClearAllAsync();
        return Ok("History cleared");
    }
}