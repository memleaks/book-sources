using System;
using System.IO;
using System.Text;
using System.CodeDom;
using System.Reflection;
using System.CodeDom.Compiler;

class TypeDeclarations
{
  static void Main()
  {
    CodeNamespace mimsyNamespace =
      new CodeNamespace("Mimsy");
    mimsyNamespace.Imports.AddRange(new[]
    {
      new CodeNamespaceImport("System"),
      new CodeNamespaceImport("System.Text"),
      new CodeNamespaceImport("System.Collections")
    });

    CodeTypeDeclaration jubjubClass =
      new CodeTypeDeclaration("Jubjub")
    {
      TypeAttributes = TypeAttributes.NotPublic
    };

    CodeMemberField wabeCountFld =
      new CodeMemberField(typeof(int), "_wabeCount")
    {
      Attributes = MemberAttributes.Private
    };
    jubjubClass.Members.Add(wabeCountFld);

    mimsyNamespace.Types.Add(jubjubClass);

    CodeConstructor jubjubCtor =
      new CodeConstructor()
    {
      Attributes = MemberAttributes.Public
    };

    var jubjubCtorParam =
      new CodeParameterDeclarationExpression(
        typeof(int), "wabeCount");
    jubjubCtor.Parameters.Add(jubjubCtorParam);

    var refWabeCountFld =
      new CodeFieldReferenceExpression(
        new CodeThisReferenceExpression(),
        "_wabeCount");
    var refWabeCountProp =
      new CodePropertyReferenceExpression(
        new CodeThisReferenceExpression(),
        "WabeCount");
    var refWabeCountArg =
      new CodeArgumentReferenceExpression("wabeCount");
    var assignWabeCount =
      new CodeAssignStatement(
        refWabeCountProp, refWabeCountArg);
    jubjubCtor.Statements.Add(assignWabeCount);

    jubjubClass.Members.Add(jubjubCtor);

    CodeMemberProperty wabeCountProp =
      new CodeMemberProperty()
    {
      Attributes = MemberAttributes.Public
        | MemberAttributes.Final,
      Type = new CodeTypeReference(typeof(int)),
      Name = "WabeCount"
    };

    wabeCountProp.GetStatements.Add(
      new CodeMethodReturnStatement(refWabeCountFld));

    var suppliedPropertyValue =
      new CodePropertySetValueReferenceExpression();
    var zero = new CodePrimitiveExpression(0);

    var suppliedPropValIsLessThanZero =
      new CodeBinaryOperatorExpression(
        suppliedPropertyValue,
        CodeBinaryOperatorType.LessThan,
        zero);

    var testSuppliedPropValAndAssign =
      new CodeConditionStatement(
        suppliedPropValIsLessThanZero,
        new CodeStatement[]
        {
          new CodeAssignStatement(
            refWabeCountFld,
            zero)
        },
        new CodeStatement[]
        {
          new CodeAssignStatement(
            refWabeCountFld,
            suppliedPropertyValue)
        });

    wabeCountProp.SetStatements.Add(
      testSuppliedPropValAndAssign);

    jubjubClass.Members.Add(wabeCountProp);

    Console.WriteLine(GenerateCSharpCodeFromNamespace(mimsyNamespace));
    Console.ReadLine();
  }

  static string GenerateCSharpCodeFromNamespace(CodeNamespace ns)
  {
    CodeGeneratorOptions genOpts = new CodeGeneratorOptions()
    {
      BracingStyle = "C",
      IndentString = "  ",
      BlankLinesBetweenMembers = false
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
