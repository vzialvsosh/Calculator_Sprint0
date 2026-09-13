namespace Core.Algorithm.Implementations;

public static class Variables
{
  public const int MaxVariableLength = 4;
  public static IReadOnlyDictionary<string, double> Vars = new Dictionary<string, double>(){
    { "e", Math.E },
    { "pi", Math.PI }
  };
}
