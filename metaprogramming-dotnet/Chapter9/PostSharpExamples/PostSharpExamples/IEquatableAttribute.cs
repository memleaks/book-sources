using System;
using System.Linq;
using System.Reflection;
using PostSharp.Aspects;
using PostSharp.Aspects.Advices;
using PostSharp.Extensibility;
using PostSharp.Reflection;

namespace PostSharpExamples
{
	public class IEquatableAttributeMock
		: InstanceLevelAspect { }

	[Serializable]
	[IntroduceInterface(typeof(IEquatable<>),
		OverrideAction = InterfaceOverrideAction.Ignore)]
	[MulticastAttributeUsage(MulticastTargets.Class,
		Inheritance = MulticastInheritance.Strict)]
	public class IEquatableAttributeMock<T>
		: IEquatable<T>
		where T : class
	{
		public object Instance { get; private set; }

		[IntroduceMember(Visibility = Visibility.Public,
			IsVirtual = true, OverrideAction = MemberOverrideAction.Ignore)]
		public bool Equals(T other)
		{
			Console.Out.WriteLine("Equals(T)");
			var areEqual = false;

			if (other != null)
			{
				var result =
					(from prop in this.Instance.GetType().GetProperties(
						BindingFlags.Instance | BindingFlags.Public)
					 where prop.CanRead
					 select prop.GetValue(this.Instance, null).Equals(prop.GetValue(other, null)))
					.Distinct().ToList();

				areEqual = result.Count != 1 ? false : result[0];
			}

			return areEqual;
		}

		[IntroduceMember(IsVirtual = true,
			OverrideAction = MemberOverrideAction.OverrideOrIgnore,
			Visibility = Visibility.Public)]
		public override bool Equals(object obj)
		{
			Console.Out.WriteLine("Equals(object)");
			return this.Instance.Equals(obj as T);
		}

		[IntroduceMember(Visibility = Visibility.Public,
			IsVirtual = true, OverrideAction = MemberOverrideAction.OverrideOrIgnore)]
		public override int GetHashCode()
		{
			return
				(from prop in this.Instance.GetType().GetProperties(
					BindingFlags.Instance | BindingFlags.Public)
				 where prop.CanRead
				 select prop.GetValue(this.Instance, null).GetHashCode())
				 .Aggregate(0, (counter, item) => counter ^= item);
		}
	}
}
