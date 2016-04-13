using System;
using PostSharp.Aspects;
using PostSharp.Aspects.Advices;
using PostSharp.Reflection;

namespace PostSharpExamples
{
	[Serializable]
	public sealed class ToStringAttribute
		: InstanceLevelAspect
	{
		[IntroduceMember(IsVirtual = true,
			OverrideAction = MemberOverrideAction.OverrideOrIgnore,
			Visibility = Visibility.Public)]
		public override string ToString()
		{
			return "PostSharp";
		}
	}
}
