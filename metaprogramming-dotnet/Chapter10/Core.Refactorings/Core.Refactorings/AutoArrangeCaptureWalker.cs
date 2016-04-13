using Roslyn.Compilers.CSharp;
using System.Collections.Generic;

namespace Core.Refactorings
{
	public sealed class AutoArrangeCaptureWalker
		: SyntaxWalker
	{
		public AutoArrangeCaptureWalker()
			: base()
		{
			this.Constructors =
				new List<ConstructorDeclarationSyntax>();
			this.Enums =
				new List<EnumDeclarationSyntax>();
			this.Events =
				new List<EventDeclarationSyntax>();
			this.Fields =
				new List<FieldDeclarationSyntax>();
			this.Methods =
				new List<MethodDeclarationSyntax>();
			this.Properties =
				new List<PropertyDeclarationSyntax>();
			this.Types =
				new List<AutoArrangeCaptureWalker>();
		}

		public override void VisitClassDeclaration(ClassDeclarationSyntax node)
		{
			var capture = new AutoArrangeCaptureWalker();
			capture.VisitTypeDeclaration(node);
			this.Types.Add(capture);
		}

		public override void VisitConstructorDeclaration(ConstructorDeclarationSyntax node)
		{
			this.Constructors.Add(node);
		}

		public override void VisitEnumDeclaration(EnumDeclarationSyntax node)
		{
			this.Enums.Add(node);
		}

		public override void VisitEventDeclaration(EventDeclarationSyntax node)
		{
			this.Events.Add(node);
		}

		public override void VisitFieldDeclaration(FieldDeclarationSyntax node)
		{
			this.Fields.Add(node);
		}

		public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
		{
			this.Methods.Add(node);
		}

		public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node)
		{
			this.Properties.Add(node);
		}

		public override void VisitStructDeclaration(StructDeclarationSyntax node)
		{
			var capture = new AutoArrangeCaptureWalker();
			capture.VisitTypeDeclaration(node);
			this.Types.Add(capture);
		}

		public void VisitTypeDeclaration(TypeDeclarationSyntax node)
		{
			this.Target = node;

			var classNode = node as ClassDeclarationSyntax;

			if (classNode != null)
			{
				base.VisitClassDeclaration(classNode);
			}
			else
			{
				base.VisitStructDeclaration(
					node as StructDeclarationSyntax);
			}
		}

		public List<ConstructorDeclarationSyntax> Constructors { get; private set; }
		public List<EnumDeclarationSyntax> Enums { get; private set; }
		public List<EventDeclarationSyntax> Events { get; private set; }
		public List<FieldDeclarationSyntax> Fields { get; private set; }
		public List<PropertyDeclarationSyntax> Properties { get; private set; }
		public List<MethodDeclarationSyntax> Methods { get; private set; }
		public TypeDeclarationSyntax Target { get; private set; }
		public List<AutoArrangeCaptureWalker> Types { get; private set; }
	}
}
