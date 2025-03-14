using System.Runtime.InteropServices;

namespace System.EnterpriseServices;

[ComVisible(false)]
[AttributeUsage(AttributeTargets.Class)]
public sealed class MustRunInClientContextAttribute : Attribute
{
	private bool val;

	public bool Value => val;

	public MustRunInClientContextAttribute()
		: this(val: true)
	{
	}

	public MustRunInClientContextAttribute(bool val)
	{
		this.val = val;
	}
}
