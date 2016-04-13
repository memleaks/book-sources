using System;

namespace SpringAOPExamples
{
	public class ClassWithData
		: IClassWithData
	{
		public ClassWithData()
			: base() { }

		public ClassWithData(Guid data)
			: base()
		{
			this.Data = data;
		}

		public Guid GetData()
		{
			return this.Data;
		}

		public Guid Data { get; private set; }
	}
}
