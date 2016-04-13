using System;
using System.Text;
using System.CodeDom.Compiler;

class ShowCompilerFeatures
{
  static void Main()
  {
    foreach (CompilerInfo ci in
      CodeDomProvider.GetAllCompilerInfo())
    {
      StringBuilder output = new StringBuilder();
      string language = ci.GetLanguages()[0];
      output.AppendFormat("{0} features:\r\n", language);
      try
      {
        CodeDomProvider provider = CodeDomProvider
          .CreateProvider(language);
        output.AppendFormat("CaseInsensitive = {0}\r\n",
          provider.LanguageOptions.HasFlag(
            LanguageOptions.CaseInsensitive));
        foreach (GeneratorSupport supportableFeature
          in Enum.GetValues(typeof(GeneratorSupport)))
        {
          output.AppendFormat("{0} = {1}\r\n",
            supportableFeature,
            provider.Supports(supportableFeature));
        }
      }
      catch (Exception ex)
      {
        output.AppendFormat("{0} occurred while getting " +
          "features for language {1} with the following " +
          "message:\r\n\r\n'{2}'.\r\n", ex.GetType().Name,
          language, ex.Message);
      }
      Console.WriteLine(output.ToString());
    }
    Console.ReadLine();
  }
}
