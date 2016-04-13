using Roslyn.Compilers.CSharp;
using Roslyn.Services;
using Roslyn.Services.Editor;
using System.Threading;
using System.Windows.Media;

namespace Wcf.Issues
{
	public sealed class OneWayOperationMakeIsOneWayFalseCodeAction
		: ICodeAction
	{
		private IDocument document;
		private AttributeArgumentSyntax attributeArgumentSyntax;

		public OneWayOperationMakeIsOneWayFalseCodeAction(
			IDocument document,
			AttributeArgumentSyntax attributeArgumentSyntax)
		{
			this.document = document;
			this.attributeArgumentSyntax = attributeArgumentSyntax;
		}

		public string Description
		{
			get { return "Make IsOneWay = false"; }
		}

		public CodeActionEdit GetEdit(
			CancellationToken cancellationToken)
		{
			var trueToken =
				this.attributeArgumentSyntax.Expression.GetFirstToken();

			var falseToken = Syntax.Token(trueToken.LeadingTrivia,
				SyntaxKind.FalseKeyword, trueToken.TrailingTrivia);

			var tree = (SyntaxTree)this.document.GetSyntaxTree();
			var newRoot = tree.GetRoot().ReplaceToken(trueToken, falseToken);
			return new CodeActionEdit(document.UpdateSyntaxRoot(newRoot));
		}

		public ImageSource Icon
		{
			get { return null; }
		}
	}
}
