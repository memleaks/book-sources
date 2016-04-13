using System;

//[assembly: PostSharpExamples.Creation]
namespace PostSharpExamples
{
	[Creation]
	public sealed class ClassWithCreation
	{
		public ClassWithCreation()
			: base() { }

		public ClassWithCreation(Guid data)
			: base()
		{
			this.Data = data;
		}

		public Guid Data { get; private set; }
	}

	public sealed class ClassWithoutCreation
	{
		public ClassWithoutCreation()
			: base() { }

		public ClassWithoutCreation(Guid data)
			: base()
		{
			this.Data = data;
		}

		public Guid Data { get; private set; }
	}
}
