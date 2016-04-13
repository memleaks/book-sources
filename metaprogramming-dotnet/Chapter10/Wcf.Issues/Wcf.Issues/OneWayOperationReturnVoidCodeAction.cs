using Roslyn.Compilers.CSharp;
using Roslyn.Services;
using Roslyn.Services.Editor;
using System.Threading;
using System.Windows.Media;

namespace Wcf.Issues
{
	public sealed class OneWayOperationReturnVoidCodeAction
		: ICodeAction
	{
		private IDocument document;
		private TypeSyntax typeSyntax;

		public OneWayOperationReturnVoidCodeAction(
			IDocument document,
			TypeSyntax typeSyntax)
		{
			this.document = document;
			this.typeSyntax = typeSyntax;
		}

		public string Description
		{
			get { return "Return System.Void"; }
		}

		public CodeActionEdit GetEdit(
			CancellationToken cancellationToken)
		{
			var returnToken = this.typeSyntax.GetFirstToken();

			var voidToken = Syntax.Token(returnToken.LeadingTrivia,
				SyntaxKind.VoidKeyword, returnToken.TrailingTrivia);

			var tree = (SyntaxTree)this.document.GetSyntaxTree();
			var newRoot = tree.GetRoot().ReplaceToken(
				returnToken, voidToken);
			return new CodeActionEdit(document.UpdateSyntaxRoot(newRoot));
		}

		public ImageSource Icon
		{
			get { return null; }
		}
	}
}
