namespace Core.Algorithm.Implementations;

public enum OperType
{
  plus,
  minus,
  mul,
  div,
  pow,

  unary_minus,
  log,
  sin,
  cos,
  tg,
  abs,
  arcsin,
  arccos,
  arctg,

  open_par,
  clos_par
}


public class Operation
{
  public static IReadOnlyDictionary<string, OperType> OperTypes = new Dictionary<string, OperType>(){
    { "+", OperType.plus },
    { "-", OperType.minus },
    { "*", OperType.mul },
    { "/", OperType.div },
    { "^", OperType.pow },

    { "abs", OperType.abs },
    { "log", OperType.log },
    { "sin", OperType.sin },
    { "cos", OperType.cos },
    { "tg", OperType.tg },
    { "arcsin", OperType.arcsin },
    { "arccos", OperType.arccos },
    { "arctg", OperType.arctg },

    { "(", OperType.open_par },
    { ")", OperType.clos_par },
  };
  public static readonly int MaxOperationLength = OperTypes.Keys.MaxBy((p) => p.Length)?.Length ?? 1;

  public readonly OperType Type;

  public Operation(OperType type)
  {
    Type = type;
  }

  public Operation(string oper, bool prevCanBeOperand=true)
  {
    if (!OperTypes.TryGetValue(oper, out var type))
      throw new ArgumentException($"Unexpected token: {oper}");
    if (type == OperType.minus && !prevCanBeOperand) Type = OperType.unary_minus;
    else Type = type;
  }

  public int Priority => Type switch
  {
    OperType.plus => 0,
    OperType.minus => 0,
    OperType.mul => 1,
    OperType.div => 1,
    OperType.pow => 2,

    OperType.unary_minus => 2,
    OperType.abs => 5,
    OperType.log => 5,
    OperType.sin => 5,
    OperType.cos => 5,
    OperType.tg => 5,
    OperType.arcsin => 5,
    OperType.arccos => 5,
    OperType.arctg => 5,
    _ => -1
  };

  public bool IsBinary => Type switch
  {
    OperType.plus => true,
    OperType.minus => true,
    OperType.mul => true,
    OperType.div => true,
    OperType.pow => true,
    _ => false
  };

  public bool IsUnary => Type switch
  {
    OperType.unary_minus => true,
    OperType.abs => true,
    OperType.log => true,
    OperType.sin => true,
    OperType.cos => true,
    OperType.tg => true,
    OperType.arcsin => true,
    OperType.arccos => true,
    OperType.arctg => true,
    _ => false
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
    OperType.unary_minus => -a,
    OperType.abs => Math.Abs(a),
    OperType.log => Math.Log(a),
    OperType.sin => Math.Sin(a),
    OperType.cos => Math.Cos(a),
    OperType.tg => Math.Tan(a),
    OperType.arcsin => Math.Asin(a),
    OperType.arccos => Math.Acos(a),
    OperType.arctg => Math.Atan(a),
    _ => throw new InvalidOperationException($"Cannot use {Type} as unary operation.")
  };
}
