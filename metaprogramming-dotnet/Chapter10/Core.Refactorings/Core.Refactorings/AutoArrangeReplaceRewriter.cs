using Roslyn.Compilers.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Refactorings
{
	public sealed class AutoArrangeReplaceRewriter
		: SyntaxRewriter
	{
		private int count;
		private List<SyntaxNode> nodes;

		public AutoArrangeReplaceRewriter(
			AutoArrangeCaptureWalker rewriter)
		{
			rewriter.Constructors.Sort(
				(a, b) => a.Identifier.ValueText.CompareTo(
					b.Identifier.ValueText));
			rewriter.Enums.Sort(
				(a, b) => a.Identifier.ValueText.CompareTo(
					b.Identifier.ValueText));
			rewriter.Events.Sort(
				(a, b) => a.Identifier.ValueText.CompareTo(
					b.Identifier.ValueText));
			rewriter.Fields.Sort(
				(a, b) => a.Declaration.Variables[0]
					.Identifier.ValueText.CompareTo(
						b.Declaration.Variables[0]
							.Identifier.ValueText));
			rewriter.Methods.Sort(
				(a, b) => a.Identifier.ValueText.CompareTo(
					b.Identifier.ValueText));
			rewriter.Properties.Sort(
				(a, b) => a.Identifier.ValueText.CompareTo(
					b.Identifier.ValueText));
			rewriter.Types.Sort(
				(a, b) => a.Target.Identifier.ValueText.CompareTo(
					b.Target.Identifier.ValueText));

			this.nodes = new List<SyntaxNode>();
			this.nodes.AddRange(rewriter.Events);
			this.nodes.AddRange(rewriter.Fields);
			this.nodes.AddRange(rewriter.Constructors);
			this.nodes.AddRange(rewriter.Methods);
			this.nodes.AddRange(rewriter.Properties);
			this.nodes.AddRange(rewriter.Enums);
			this.nodes.AddRange(
				from typeRewriter in rewriter.Types
				select new AutoArrangeReplaceRewriter(typeRewriter)
					.VisitTypeDeclaration(typeRewriter.Target)
						as TypeDeclarationSyntax);
		}

		private SyntaxNode Replace(SyntaxNode node)
		{
			SyntaxNode result = null;

			if (this.count < this.nodes.Count)
			{
				result = this.nodes[this.count];
				this.count++;
			}
			else
			{
				throw new NotSupportedException();
			}

			return result;
		}

		public override SyntaxNode VisitClassDeclaration(ClassDeclarationSyntax node)
		{
			return this.Replace(node);
		}

		public override SyntaxNode VisitConstructorDeclaration(ConstructorDeclarationSyntax node)
		{
			return this.Replace(node);
		}

		public override SyntaxNode VisitEnumDeclaration(EnumDeclarationSyntax node)
		{
			return this.Replace(node);
		}

		public override SyntaxNode VisitEventDeclaration(EventDeclarationSyntax node)
		{
			return this.Replace(node);
		}

		public override SyntaxNode VisitFieldDeclaration(FieldDeclarationSyntax node)
		{
			return this.Replace(node);
		}

		public override SyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax node)
		{
			return this.Replace(node);
		}

		public override SyntaxNode VisitPropertyDeclaration(PropertyDeclarationSyntax node)
		{
			return this.Replace(node);
		}

		public override SyntaxNode VisitStructDeclaration(StructDeclarationSyntax node)
		{
			return this.Replace(node);
		}

		public TypeDeclarationSyntax VisitTypeDeclaration(
			TypeDeclarationSyntax node)
		{
			var classNode = node as ClassDeclarationSyntax;

			if (classNode != null)
			{
				return base.VisitClassDeclaration(classNode)
					as ClassDeclarationSyntax;
			}
			else
			{
				return base.VisitStructDeclaration(
					node as StructDeclarationSyntax)
					as StructDeclarationSyntax;
			}
		}
	}
}
