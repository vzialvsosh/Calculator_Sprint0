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
    return Calculate(_parser.Parse(s));
  }

  void ApplyBinaryOperation(Stack<Operation> operations, Stack<double> operands)
  {
    Operation prevOper = operations.Peek();
    operations.Pop();

    if (prevOper.Type == OperType.open_par) return;
    if (operands.Count < 2) throw new ArgumentException("Invalid expression.");

    double b = operands.Peek();
    operands.Pop();
    double a = operands.Peek();
    operands.Pop();
    operands.Push(prevOper.ApplyAsBinary(a, b));
  }

  void ApplyUnaryOperation(Operation oper, double x, Stack<double> operands)
  {
    operands.Push(oper.ApplyAsUnary(x));
  }

  double Calculate(List<string> tokens)
  {
    Stack<Operation> operations = new Stack<Operation>();
    Stack<double> operands = new Stack<double>();

    bool prevCanBeOperand = false;
    for (int i = 0; i < tokens.Count; ++i)
    {
      if (double.TryParse(tokens[i], NumberStyles.Any, CultureInfo.InvariantCulture, out double x))
      {
        operands.Push(x);
        prevCanBeOperand = true;
        continue;
      }

      Operation oper = new Operation(tokens[i]);
      if (oper.Type == OperType.minus && !prevCanBeOperand)
      {
        ++i;
        if (i >= tokens.Count) throw new ArgumentException("Invalid expression: unary minus is not followed by a number.");
        if (!double.TryParse(tokens[i], out x)) throw new ArgumentException("Invalid expression: unary minus is not followed by a number.");
        ApplyUnaryOperation(oper, x, operands);
        prevCanBeOperand = true;
        continue;
      }

      if (oper.Type == OperType.clos_par)
      {
        while (operations.Count > 0 && operations.Peek().Type != OperType.open_par)
          ApplyBinaryOperation(operations, operands);
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
      while (operations.Count > 0 && (operations.Peek().Priority() > oper.Priority() ||
      (oper.Type != OperType.pow && operations.Peek().Priority() == oper.Priority())))
        ApplyBinaryOperation(operations, operands);
      operations.Push(oper);
      prevCanBeOperand = false;
    }

    while (operations.Count > 0)
      ApplyBinaryOperation(operations, operands);

    if (operands.Count != 1) throw new ArgumentException("Invalid expression.");

    return operands.First();
  }
}
