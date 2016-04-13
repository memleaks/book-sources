using Roslyn.Compilers.CSharp;
using Roslyn.Services;
using Roslyn.Services.Editor;
using System.Threading;
using System.Windows.Media;

namespace Core.Refactorings
{
	public sealed class AutoArrangeCodeAction
		: ICodeAction
	{
		private IDocument document;
		private TypeDeclarationSyntax token;

		public AutoArrangeCodeAction(IDocument document, TypeDeclarationSyntax token)
		{
			this.document = document;
			this.token = token;
		}

		public string Description
		{
			get { return "Auto-arrange"; }
		}

		public CodeActionEdit GetEdit(
			CancellationToken cancellationToken)
		{
			var captureWalker = new AutoArrangeCaptureWalker();
			captureWalker.VisitTypeDeclaration(this.token);
			var result = new AutoArrangeReplaceRewriter(
				captureWalker).VisitTypeDeclaration(this.token);

			var tree = (SyntaxNode)this.document.GetSyntaxRoot(
				cancellationToken);
			var newTree = tree.ReplaceNodes(new [] { this.token },
				(a, b) => result);
			return new CodeActionEdit(document.UpdateSyntaxRoot(newTree));
		}

		public ImageSource Icon
		{
			get { return null; }
		}
	}
}
