using System;
using System.Collections.Generic;

class Chapter3Intro
{
  public static void GenerateDataClass(string className,
    List<Tuple<Type, string, bool>> properties,
    bool generateCtor = true)
  {
    Console.WriteLine("public class {0}", className);
    Console.WriteLine("{");
    foreach (var property in properties)
    {
      Console.WriteLine(
        "  public {0} {1}() {{ get; {2}set; }}",
        property.Item1.FullName,
        property.Item2,
        property.Item3 ? "" : "private ");
    }
    if (generateCtor)
    {
      Console.Write("  public {0}(", className);
      for (int ndx = 0; ndx < properties.Count; ndx++)
        Console.Write("{0}{1} {2}",
          (ndx > 0) ? ", " : "",
          properties[ndx].Item1,
          properties[ndx].Item2,
          properties[ndx].Item3);
      Console.WriteLine(")");
      Console.WriteLine("  {");
      foreach (var property in properties)
      {
        Console.WriteLine(
          "    this.{0} = {0};",
          property.Item2);
      }
      Console.WriteLine("  }");
    }
    Console.WriteLine("}");
  }
}