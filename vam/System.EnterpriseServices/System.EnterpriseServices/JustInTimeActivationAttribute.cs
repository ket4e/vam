using System.Runtime.InteropServices;

namespace System.EnterpriseServices;

[ComVisible(false)]
[AttributeUsage(AttributeTargets.Class)]
public sealed class JustInTimeActivationAttribute : Attribute
{
	private bool val;

	public bool Value => val;

	public JustInTimeActivationAttribute()
		: this(val: true)
	{
	}

	public JustInTimeActivationAttribute(bool val)
	{
		this.val = val;
	}
}
