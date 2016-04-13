using Roslyn.Compilers;
using Roslyn.Compilers.Common;
using Roslyn.Compilers.CSharp;
using Roslyn.Services;
using Roslyn.Services.Editor;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using System.ServiceModel;

namespace Wcf.Issues
{
	[ExportSyntaxNodeCodeIssueProvider(
		"Wcf.Issues.OneWayOperationIssueProvider",
		LanguageNames.CSharp,
		typeof(MethodDeclarationSyntax))]
	public sealed class OneWayOperationCodeIssueProvider
		: ICodeIssueProvider
	{
		public IEnumerable<CodeIssue> GetIssues(IDocument document,
			CommonSyntaxNode node,
			CancellationToken cancellationToken)
		{
			var methodNode = (MethodDeclarationSyntax)node;

			if (methodNode.Attributes != null)
			{
				var model = document.GetSemanticModel();
				var operationContractType = 
					typeof(OperationContractAttribute);
				var operationSyntax = (
					from attribute in methodNode.Attributes
					from syntax in attribute.Attributes
					let attributeType = model.GetTypeInfo(syntax).Type
					where 
						attributeType != null &&
						attributeType.Name == 
							operationContractType.Name &&
						attributeType.ContainingAssembly.Name == 
							operationContractType.Assembly.GetName().Name
					from argument in syntax.ArgumentList.Arguments
					where (
						argument.NameEquals.Identifier.GetText() ==
							"IsOneWay" &&
						argument.Expression.Kind ==
							SyntaxKind.TrueLiteralExpression)
					select new { syntax, argument }).FirstOrDefault();

				if (operationSyntax != null)
				{
					var returnType = model.GetTypeInfo(methodNode.ReturnType).Type;

					if (returnType != null &&
						returnType.SpecialType != SpecialType.System_Void)
					{
						return new[] 
						{
							new CodeIssue(CodeIssue.Severity.Error, 
								methodNode.ReturnType.Span,
								"One-way WCF operations must return System.Void.",
								new ICodeAction[] 
								{ 
									new OneWayOperationReturnVoidCodeAction(
										document, 
										methodNode.ReturnType),
									new OneWayOperationMakeIsOneWayFalseCodeAction(
										document, 
										operationSyntax.argument)
								})
						};
					}
				}
			}

			return null;
		}

		public IEnumerable<CodeIssue> GetIssues(IDocument document,
			CommonSyntaxToken token,
			CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<CodeIssue> GetIssues(IDocument document,
			CommonSyntaxTrivia trivia,
			CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}
	}
}
