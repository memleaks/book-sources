using Roslyn.Compilers;
using Roslyn.Scripting;
using Roslyn.Scripting.CSharp;
using System.Linq;
using System.Reflection;

namespace ToStringWithRoslyn
{
	public static class ToStringViaRoslynExtensions
	{
		public sealed class Host<T>
		{
			public Host(T target)
			{
				this.Target = target;
			}

			public T Target { get; private set; }
		}

		public static string Generate<T>(this T @this)
		{
			var code = "new StringBuilder()" +
				string.Join(".Append(\" || \")",
					from property in @this.GetType().GetProperties(
						BindingFlags.Instance | BindingFlags.Public)
					where property.CanRead
					select string.Format(".Append(\"{0}: \").Append(Target.{0})",
						property.Name)) + ".ToString()";

			var hostReference =
				new AssemblyFileReference(typeof(ToStringViaRoslynExtensions).Assembly.Location);
			var engine = new ScriptEngine(
				references: new[] { hostReference },
				importedNamespaces: new[] { "System", "System.Text" });
			var host = new Host<T>(@this);
			var session = Session.Create(host);
			return engine.Execute<string>(code, session);
		}
	}
}
