using System.Runtime.InteropServices;

namespace System.EnterpriseServices;

[AttributeUsage(AttributeTargets.Class)]
[ComVisible(false)]
public sealed class COMTIIntrinsicsAttribute : Attribute
{
	private bool val;

	public bool Value => val;

	public COMTIIntrinsicsAttribute()
	{
		val = false;
	}

	public COMTIIntrinsicsAttribute(bool val)
	{
		this.val = val;
	}
}
