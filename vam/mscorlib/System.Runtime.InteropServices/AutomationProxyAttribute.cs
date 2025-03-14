namespace System.Runtime.InteropServices;

[ComVisible(true)]
[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Interface, Inherited = false)]
public sealed class AutomationProxyAttribute : Attribute
{
	private bool val;

	public bool Value => val;

	public AutomationProxyAttribute(bool val)
	{
		this.val = val;
	}
}
