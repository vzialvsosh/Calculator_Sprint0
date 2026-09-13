using System.Text;
using Core.Algorithm.Interfaces;

namespace Core.Algorithm.Implementations;

public class Parser : IParser
{
  bool IsPartOfDouble(char c)
  {
    if (char.IsDigit(c)) return true;
    if (c == '.' || c == ',') return true;
    return false;
  }

  List<string> ParseWithoutSpaces(string s)
  {
    List<string> tokens = new List<string>();

    int i = 0, j = 0;
    while (j < s.Length)
    {
      ++j;
      if (IsPartOfDouble(s[i]))
        while (j < s.Length && IsPartOfDouble(s[j]))
          ++j;
      else
        while (j < s.Length && j - i <= Operation.MaxOperationLength && !Operation.OperTypes.ContainsKey(s[i..j]))
          ++j;
      tokens.Add(s[i..j]);
      i = j;
    }
    return tokens;
  }

  string Preprocess(string s)
  {
    StringBuilder res = new StringBuilder();
    for (int i = 0; i < s.Length; ++i)
    {
      if (char.IsWhiteSpace(s[i])) continue;
      if (s[i] == ',') res.Append('.');
      else res.Append(s[i]);
    }
    return res.ToString();
  }

  public List<string> Parse(string s) => ParseWithoutSpaces(Preprocess(s));
}
