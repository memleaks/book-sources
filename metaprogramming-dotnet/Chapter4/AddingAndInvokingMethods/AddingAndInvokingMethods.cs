using System;
using System.IO;
using System.Text;
using System.CodeDom;
using System.Reflection;
using System.Collections;
using System.CodeDom.Compiler;

class AddingAndInvokingMethods
{
  static void Main()
  {
    CodeNamespace mimsyNamespace =
      CreateMimsyNamespace();
    Console.WriteLine(
      GenerateCSharpCodeFromNamespace(
        mimsyNamespace));
    Console.ReadLine();
  }

  static CodeNamespace CreateMimsyNamespace()
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
        TypeAttributes = TypeAttributes.Public
      };

    CodeMemberField wabeCountFld =
      new CodeMemberField(typeof(int), "_wabeCount")
      {
        Attributes = MemberAttributes.Private
      };
    jubjubClass.Members.Add(wabeCountFld);

    var typrefArrayList =
      new CodeTypeReference("ArrayList");

    CodeMemberField updatesFld =
      new CodeMemberField(typrefArrayList, "_updates");
    jubjubClass.Members.Add(updatesFld);

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

    var refUpdatesFld =
      new CodeFieldReferenceExpression(
        new CodeThisReferenceExpression(),
        "_updates");
    var newArrayList =
      new CodeObjectCreateExpression(typrefArrayList);
    var assignUpdates =
      new CodeAssignStatement(
        refUpdatesFld, newArrayList);
    jubjubCtor.Statements.Add(assignUpdates);

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

    wabeCountProp.SetStatements.Add(
      new CodeMethodInvokeExpression(
        new CodeMethodReferenceExpression(
          refUpdatesFld,
          "Add"),
        refWabeCountFld));

    jubjubClass.Members.Add(wabeCountProp);

    CodeMemberMethod methGetWabeCountHistory =
      new CodeMemberMethod()
      {
        Attributes = MemberAttributes.Public
         | MemberAttributes.Final,
        Name = "GetWabeCountHistory",
        ReturnType = new CodeTypeReference(typeof(String))
      };
    jubjubClass.Members.Add(methGetWabeCountHistory);

    methGetWabeCountHistory.Statements.Add(
      new CodeVariableDeclarationStatement(
        "StringBuilder", "result"));
    var refResultVar =
      new CodeVariableReferenceExpression("result");
    methGetWabeCountHistory.Statements.Add(
      new CodeAssignStatement(
        refResultVar,
        new CodeObjectCreateExpression(
          "StringBuilder")));

    methGetWabeCountHistory.Statements.Add(
      new CodeVariableDeclarationStatement(
        typeof(int), "ndx"));
    var refNdxVar =
      new CodeVariableReferenceExpression("ndx");

    methGetWabeCountHistory.Statements.Add(
      new CodeIterationStatement(
        new CodeAssignStatement(
          refNdxVar,
          new CodePrimitiveExpression(0)),
        new CodeBinaryOperatorExpression(
          refNdxVar,
          CodeBinaryOperatorType.LessThan,
          new CodePropertyReferenceExpression(
            refUpdatesFld,
            "Count")),
        new CodeAssignStatement(
          refNdxVar,
          new CodeBinaryOperatorExpression(
            refNdxVar,
            CodeBinaryOperatorType.Add,
            new CodePrimitiveExpression(1))),
        new CodeConditionStatement(
          new CodeBinaryOperatorExpression(
            refNdxVar,
            CodeBinaryOperatorType.ValueEquality,
            new CodePrimitiveExpression(0)),
          new CodeStatement[]
          {
            new CodeExpressionStatement(
              new CodeMethodInvokeExpression(
                new CodeMethodReferenceExpression(
                  refResultVar,
                  "AppendFormat"),
                new CodePrimitiveExpression("{0}"),
                new CodeArrayIndexerExpression(
                  refUpdatesFld,
                  refNdxVar)))
          },
          new CodeStatement[]
          {
            new CodeExpressionStatement(
              new CodeMethodInvokeExpression(
                new CodeMethodReferenceExpression(
                  refResultVar,
                  "AppendFormat"),
                new CodePrimitiveExpression(", {0}"),
                new CodeArrayIndexerExpression(
                  refUpdatesFld,
                  refNdxVar)))
          })));

    methGetWabeCountHistory.Statements.Add(
      new CodeMethodReturnStatement(
        new CodeMethodInvokeExpression(
          new CodeMethodReferenceExpression(
            refResultVar, "ToString"))));

    return mimsyNamespace;
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
