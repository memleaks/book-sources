using System;
using System.Linq;
using System.Reflection;
using PostSharp.Aspects;
using PostSharp.Aspects.Advices;
using PostSharp.Reflection;

namespace PostSharpExamples
{
	[Serializable]
	public sealed class EqualsAttribute
		: InstanceLevelAspect
	{
		[IntroduceMember(IsVirtual = true,
			OverrideAction = MemberOverrideAction.OverrideOrIgnore,
			Visibility = Visibility.Public)]
		public override bool Equals(object obj)
		{
			var areEqual = false;

			if (obj != null && this.Instance.GetType()
				.IsAssignableFrom(obj.GetType()))
			{
				var result =
					(from prop in this.Instance.GetType().GetProperties(
						BindingFlags.Instance | BindingFlags.Public)
					 where prop.CanRead
					 select prop.GetValue(this.Instance, null)
						.Equals(prop.GetValue(obj, null)))
					.Distinct().ToList();

				areEqual = result.Count != 1 ? false : result[0];
			}

			return areEqual;
		}

		[IntroduceMember(IsVirtual = true,
			OverrideAction = MemberOverrideAction.OverrideOrIgnore,
			Visibility = Visibility.Public)]
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
