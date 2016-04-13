using System;
using Spring.Aop.Framework;

namespace SpringAOPExamples
{
	public static class Program
	{
		private static void Main(string[] args)
		{
			var factoryNoData = new ProxyFactory(new ClassWithData());
			factoryNoData.AddAdvice(new PropertyInterceptor());
			var noDataWithInterceptor = (IClassWithData)factoryNoData.GetProxy();
			Console.Out.WriteLine(noDataWithInterceptor.Data);
			//Console.Out.WriteLine(noDataWithInterceptor.GetData());

			//Console.Out.WriteLine();

			//var factoryData = new ProxyFactory(new ClassWithData(Guid.NewGuid()));
			//factoryData.AddAdvice(new PropertyInterceptor());
			//var dataWithInterceptor = (IClassWithData)factoryData.GetProxy();
			//Console.Out.WriteLine(dataWithInterceptor.Data);
			//Console.Out.WriteLine(dataWithInterceptor.GetData());
		}
	}
}
