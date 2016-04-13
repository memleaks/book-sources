using Roslyn.Compilers;
using Roslyn.Compilers.CSharp;
using Roslyn.Services;
using Roslyn.Services.Editor;
using System.ComponentModel.Composition;
using System.Threading;

namespace Core.Refactorings
{
	[ExportCodeRefactoringProvider(
		"Core.Refactorings.AutoAlphabetizeCodeRefactoringProvider",
		LanguageNames.CSharp)]
	public sealed class AutoArrangeCodeRefactoringProvider
		: ICodeRefactoringProvider
	{
		public CodeRefactoring GetRefactoring(IDocument document,
			TextSpan textSpan, CancellationToken cancellationToken)
		{
			var token = document.GetSyntaxTree(cancellationToken)
				.GetRoot().FindToken(textSpan.Start);
			var parent = token.Parent;

			if (parent != null && (
				parent.Kind == (int)SyntaxKind.ClassDeclaration ||
				parent.Kind == (int)SyntaxKind.StructDeclaration))
			{
				return new CodeRefactoring(
					new[] 
					{ 
						new AutoArrangeCodeAction(document, 
							parent as TypeDeclarationSyntax)
					});
			}

			return null;
		}
	}
}
