using System.Runtime.InteropServices;

namespace System.EnterpriseServices;

[ComVisible(false)]
[AttributeUsage(AttributeTargets.Class)]
public sealed class EventTrackingEnabledAttribute : Attribute
{
	private bool val;

	public bool Value => val;

	public EventTrackingEnabledAttribute()
	{
		val = true;
	}

	public EventTrackingEnabledAttribute(bool val)
	{
		this.val = val;
	}
}
