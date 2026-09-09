namespace Algorithm;

public class Calculator : ICalculator
{
  Parser _parser;
  public Calculator(Parser parser)
  {
    _parser = parser;
  }

  // enum Oper
  // {
  //   plus,
  //   minus,
  //   mul,
  //   dev,
  //   pow,
  //   // log,
  //   open_par,
  //   clos_par
  // }

  // int Priority(Oper oper) => oper switch
  // {
  //   Oper.plus => 0,
  //   Oper.minus => 0,
  //   Oper.mul => 1,
  //   Oper.dev => 1,
  //   Oper.pow => 2,
  //   _ => -1
  // };

  // // bool IsUnary(Oper oper) => oper switch
  // // {
  // //   Oper.log => true,
  // //   _ => false
  // // };

  // Oper OperFromString(string oper) => oper switch
  // {
  //   "+" => Oper.plus,
  //   "-" => Oper.minus,
  //   "*" => Oper.mul,
  //   "/" => Oper.dev,
  //   "^" => Oper.pow,
  //   // "log" => Oper.log,
  //   "(" => Oper.open_par,
  //   ")" => Oper.clos_par,
  //   _ => throw new ArgumentOutOfRangeException($"Unexpected token: {oper}")
  // };

  // double Apply(double a, double b, Oper oper) => oper switch
  // {
  //   Oper.plus => a + b,
  //   Oper.minus => a - b,
  //   Oper.mul => a * b,
  //   Oper.dev => a / b,
  //   Oper.pow => Math.Pow(a, b),
  //   _ => throw new Exception()
  // };

  public double Calculate(string s)
  {
    try
    {
      return Calculate(_parser.Parse(s));
    }
    catch
    {
      // throw new InvalidOperationException("Invalid expression.");
      throw;
    }
  }

  void Apply(Stack<Operation> operations, Stack<double> operands)
  {
    Operation prevOper = operations.Peek();
    operations.Pop();
    if (prevOper.Type == OperType.open_par) return;

    double b = operands.Peek();
    operands.Pop();
    double a = operands.Peek();
    operands.Pop();
    operands.Push(prevOper.Apply(a, b));
  }

  double Calculate(List<string> tokens)
  {
    Stack<Operation> operations = new Stack<Operation>();
    Stack<double> operands = new Stack<double>();

    for (int i = 0; i < tokens.Count; ++i)
    {
      if (double.TryParse(tokens[i], out double x))
      {
        operands.Push(x);
        continue;
      }
      Operation oper = new Operation(tokens[i]);
      if (oper.Type == OperType.clos_par)
      {
        while (operations.Peek().Type != OperType.open_par)
          Apply(operations, operands);
        operations.Pop();
        continue;
      }
      if (oper.Type == OperType.open_par)
      {
        operations.Push(oper);
        continue;
      }
      while (operations.Count > 0 && operations.Peek().Priority() >= oper.Priority())
        Apply(operations, operands);
      operations.Push(oper);
    }

    while (operations.Count > 0)
      Apply(operations, operands);
      
    if (operands.Count != 1) throw new Exception();

    return operands.First();
  }
}
