using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.Collections.Generic;

class IntantiatingCodeProviders
{
  static void Main()
  {
    var providerOptions = new Dictionary<string, string>();
    providerOptions.Add("CompilerVersion", "v4.0");
    var csProv = new CSharpCodeProvider(providerOptions);
    var compilerParameters =
      new CompilerParameters(new string[] { });

    CompilerResults results =
      csProv.CompileAssemblyFromSource(compilerParameters,
@"namespace V3Features
{
  class Program {
    static void Main() {
      var name = ""Kevin"";
      System.Console.WriteLine(name);
    }
  }
}");
  }
}
