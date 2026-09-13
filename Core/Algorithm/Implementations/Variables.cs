namespace Core.Algorithm.Implementations;

public static class Variables
{
  public static IReadOnlyDictionary<string, double> Vars = new Dictionary<string, double>(){
    { "e", Math.E },
    { "pi", Math.PI }
  };
  public static readonly int MaxVariableLength = Vars.Keys.MaxBy((k) => k.Length)?.Length ?? 1;
}
