using System;

namespace SpringAOPExamples
{
	public interface IClassWithData
	{
		Guid GetData();
		Guid Data { get; }
	}
}
