using Microsoft.AspNetCore.Mvc;
using Core.Algorithm.Interfaces;

namespace Backend.Controllers;

[ApiController]
[Route("api/calculator")]
public class CalculatorController : ControllerBase
{
    private readonly ICalculator _calculator;

    public CalculatorController(ICalculator calculator)
    {
        _calculator = calculator;
    }

    [HttpPost("calculate")]
    public IActionResult Calculate([FromBody] string expression)
    {
        try
        {
            double result = _calculator.Calculate(expression);
            return Ok(new { result });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}