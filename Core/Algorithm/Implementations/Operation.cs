namespace Algorithm;

public enum OperType
{
  plus,
  minus,
  mul,
  dev,
  pow,
  // log,
  open_par,
  clos_par
}

public class Operation
{
  public readonly static Dictionary<string, OperType> OperTypes = new(){
    { "+", OperType.plus },
    { "-", OperType.minus },
    { "*", OperType.mul },
    { "/", OperType.dev },
    { "^", OperType.pow },
    { "(", OperType.open_par },
    { ")", OperType.clos_par },
  };

  public readonly OperType Type;

  public Operation(OperType type)
  {
    Type = type;
  }

  public Operation(string oper)
  {
    Type = OperTypes[oper];
  }

  public int Priority() => Type switch
  {
    OperType.plus => 0,
    OperType.minus => 0,
    OperType.mul => 1,
    OperType.dev => 1,
    OperType.pow => 2,
    _ => -1
  };

  public double Apply(double a, double b) => Type switch
  {
    OperType.plus => a + b,
    OperType.minus => a - b,
    OperType.mul => a * b,
    OperType.dev => a / b,
    OperType.pow => Math.Pow(a, b),
    _ => throw new Exception()
  };
}
