using System.Globalization;
using Core.Algorithm.Interfaces;

namespace Core.Algorithm.Implementations;

public class Calculator : ICalculator
{
  private readonly IParser _parser;

  public Calculator(IParser parser)
  {
    _parser = parser;
  }

  public double Calculate(string s)
  {
    double result = Calculate(_parser.Parse(s));
    if (result == double.NaN || result == double.PositiveInfinity || result == double.NegativeInfinity)
      throw new InvalidOperationException("Arithmetical error occured while calculating.");
    return result;
  }

  void ApplyBinaryOperation(Operation oper, Stack<double> operands)
  {
    if (operands.Count < 2) throw new ArgumentException("Invalid expression.");

    double b = operands.Peek();
    operands.Pop();
    double a = operands.Peek();
    operands.Pop();
    operands.Push(oper.ApplyAsBinary(a, b));
  }

  void ApplyUnaryOperation(Operation oper, Stack<double> operands)
  {
    if (operands.Count < 1) throw new ArgumentException("Invalid expression.");

    double a = operands.Peek();
    operands.Pop();
    operands.Push(oper.ApplyAsUnary(a));
  }

  void Apply(Stack<Operation> operations, Stack<double> operands)
  {
    Operation oper = operations.Peek();
    operations.Pop();
    if (oper.IsUnary) ApplyUnaryOperation(oper, operands);
    else if (oper.IsBinary) ApplyBinaryOperation(oper, operands);
  }

  double Calculate(List<string> tokens)
  {
    Stack<Operation> operations = new Stack<Operation>();
    Stack<double> operands = new Stack<double>();

    bool prevCanBeOperand = false;
    for (int i = 0; i < tokens.Count; ++i)
    {
      if (double.TryParse(tokens[i], NumberStyles.Any, CultureInfo.InvariantCulture, out double x) ||
        Variables.Vars.TryGetValue(tokens[i], out x))
      {
        operands.Push(x);
        prevCanBeOperand = true;
        continue;
      }

      Operation oper = new Operation(tokens[i], prevCanBeOperand);

      if (oper.Type == OperType.clos_par)
      {
        while (operations.Count > 0 && operations.Peek().Type != OperType.open_par)
          Apply(operations, operands);
        if (operations.Count == 0) throw new ArgumentException("Unpaired parenthesis ')'.");
        operations.Pop();
        prevCanBeOperand = true;
        continue;
      }
      if (oper.Type == OperType.open_par)
      {
        operations.Push(oper);
        prevCanBeOperand = false;
        continue;
      }
      while (operations.Count > 0 && (operations.Peek().Priority > oper.Priority ||
      (oper.Type != OperType.pow && !oper.IsUnary && operations.Peek().Priority == oper.Priority)))
        Apply(operations, operands);
      operations.Push(oper);
      prevCanBeOperand = false;
    }

    while (operations.Count > 0)
      Apply(operations, operands);

    if (operands.Count != 1) throw new ArgumentException("Invalid expression.");

    return operands.Peek();
  }
}
