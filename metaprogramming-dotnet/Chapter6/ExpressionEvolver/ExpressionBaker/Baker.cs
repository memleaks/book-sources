using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ExpressionBaker
{
	public sealed class Baker<TDelegate>
	{
		public Baker(string expression)
		{
			if(string.IsNullOrWhiteSpace(expression))
			{
				throw new ArgumentException(
					"An expressions must be given.", "expression");
			}

			this.Expression = expression;
		}

		public Expression<TDelegate> Bake()
		{
			var name = string.Format(CultureInfo.CurrentCulture, 
				BakerConstants.Name, Guid.NewGuid().ToString("N"));
			var cscFileName = name + ".cs";
			File.WriteAllText(cscFileName,
				string.Format(CultureInfo.CurrentCulture,
					BakerConstants.Program, name, this.GetDelegateType(), 
					this.Expression));

			this.CreateAssembly(name, cscFileName);

			return Assembly.LoadFrom(name + ".dll").GetType(name)
				.GetField("func").GetValue(null) as Expression<TDelegate>;
		}

		private void CreateAssembly(string name, string cscFileName)
		{
			var startInformation = new ProcessStartInfo("csc");
			startInformation.CreateNoWindow = true;
			startInformation.Arguments = string.Format(CultureInfo.CurrentCulture,
				BakerConstants.CscArguments, name, cscFileName);
			startInformation.RedirectStandardOutput = true;
			startInformation.UseShellExecute = false;

			var csc = Process.Start(startInformation);
			csc.WaitForExit();
		}

		private string GetDelegateType()
		{
			var type = typeof(TDelegate);

			var name = type.Name.Split('`')[0];

			return string.Format(CultureInfo.CurrentCulture, 
				BakerConstants.GenericArguments, name, 
				string.Join(",", 
					(from argument in type.GetGenericArguments() select argument.Name)));
		}

		public string Expression { get; private set; }

		private static class BakerConstants
		{
			public const string GenericArguments = "{0}<{1}>";
			public const string Name = "Baker{0}";
			public const string Program =
				@"using System;using System.Linq.Expressions;
				public static class {0}{{
				public static readonly Expression<{1}> func = {2};}}";
			public const string CscArguments = "/target:library /out:{0}.dll {1}";
		}
	}
}
