using System;
using PostSharp.Aspects;

namespace PostSharpExamples
{
	[Serializable]
	public sealed class CreationAttribute
		: OnMethodBoundaryAspect
	{
		public override void OnEntry(MethodExecutionArgs args)
		{
			if (args.Method.IsConstructor)
			{
				Console.Out.WriteLine(
					"Object {0} was instantiated with the following arguments:",
					args.Method.DeclaringType.Name);

				foreach (var argument in args.Arguments)
				{
					Console.Out.WriteLine("Type: {0} || Value: {1}",
						argument.GetType().Name, argument);
				}
			}
		}
	}
}
