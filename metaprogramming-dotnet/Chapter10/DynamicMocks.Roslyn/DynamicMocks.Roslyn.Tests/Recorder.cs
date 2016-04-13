using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DynamicMocks.Roslyn.Tests
{
	public abstract class Recorder
	{
		protected Recorder()
			: base()
		{
			this.Callbacks = new List<string>();
		}

		public ReadOnlyCollection<string> GetCallbacks()
		{
			return this.Callbacks.AsReadOnly();
		}

		protected List<string> Callbacks { get; private set; }
	}
}
