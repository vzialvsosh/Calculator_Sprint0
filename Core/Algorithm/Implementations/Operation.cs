namespace Core.Algorithm.Implementations;

public enum OperType
{
  plus,
  minus,
  mul,
  div,
  pow,

  // log,
  // sin,
  // cos,
  // tg,
  // abs,

  open_par,
  clos_par
}


public class Operation
{
  public const int MaxOperationLength = 5;
  public static IReadOnlyDictionary<string, OperType> OperTypes = new Dictionary<string, OperType>(){
    { "+", OperType.plus },
    { "-", OperType.minus },
    { "*", OperType.mul },
    { "/", OperType.div },
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
    if (!OperTypes.TryGetValue(oper, out var type))
      throw new ArgumentException($"Unexpected token: {oper}");
    Type = type;
  }

  public int Priority() => Type switch
  {
    OperType.plus => 0,
    OperType.minus => 0,
    OperType.mul => 1,
    OperType.div => 1,
    OperType.pow => 2,
    _ => -1
  };

  public double ApplyAsBinary(double a, double b) => Type switch
  {
    OperType.plus => a + b,
    OperType.minus => a - b,
    OperType.mul => a * b,
    OperType.div => a / b,
    OperType.pow => Math.Pow(a, b),
    _ => throw new InvalidOperationException($"Cannot use {Type} as binary operation.")
  };

  public double ApplyAsUnary(double a) => Type switch
  {
    OperType.minus => -a,
    _ => throw new InvalidOperationException($"Cannot use {Type} as binary operation.")
  };
}
