using System;
using System.IO;
using System.Text;
using System.CodeDom;
using System.CodeDom.Compiler;

class NamespacesAndImports
{
  static void Main()
  {
    CodeNamespace ns = new CodeNamespace("Mimsy.Borogove");
    ns.Imports.AddRange(new[]
    {
      new CodeNamespaceImport("System"),
      new CodeNamespaceImport("System.Text")
    });
    Console.WriteLine(GenerateCSharpCodeFromNamespace(ns));
    Console.ReadLine();
  }

  static string GenerateCSharpCodeFromNamespace(CodeNamespace ns)
  {
    CodeGeneratorOptions genOpts = new CodeGeneratorOptions()
    {
      BracingStyle = "C",
      IndentString = "  ",
    };
    StringBuilder gennedCode = new StringBuilder();
    using (StringWriter sw = new StringWriter(gennedCode))
    {
      CodeDomProvider.CreateProvider("c#")
        .GenerateCodeFromNamespace(ns, sw, genOpts);
    }
    return gennedCode.ToString();
  }
}
