using Spackle.Extensions;
using System;

namespace ExpressionEvolver
{
	public sealed class ExpressionEvolverResult
	{
		public ExpressionEvolverResult(double parameter, double result)
			: base()
		{
			this.Parameter = parameter;
			this.Result = result;
		}

		public ExpressionEvolverResult(double parameter, ArithmeticException exception)
			: base()
		{
			exception.CheckParameterForNull("exception");

			this.Parameter = parameter;
			this.Exception = exception;
		}

		public ArithmeticException Exception { get; private set; }
		public double Parameter { get; private set; }
		public double Result { get; private set; }
	}
}
