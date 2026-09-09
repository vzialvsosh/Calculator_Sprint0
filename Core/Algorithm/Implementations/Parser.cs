using System.Text;

namespace Algorithm;

public class Parser : IParser
{
  public List<string> Pars(string s)
  {
    return ["(", "2", "+", "3", ")", "^", "2", "*", "4", "-", "(", "8", "/", "4", ")", "+", "1"];
  }

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
        while (j < s.Length && !Operation.OperTypes.ContainsKey(s.Substring(i, j - i)))
          ++j;
      tokens.Add(s.Substring(i, j - i));
      i = j;
    }
    return tokens;
  }

  string SkipSpaces(string s)
  {
    StringBuilder res = new StringBuilder();
    for (int i = 0; i < s.Length; ++i)
    {
      if (s[i] != ' ') res.Append(s[i]);
    }
    return res.ToString();
  }

  public List<string> Parse(string s) => ParseWithoutSpaces(SkipSpaces(s));
}
