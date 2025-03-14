using System.Runtime.InteropServices;

namespace System.EnterpriseServices.CompensatingResourceManager;

[ProgId("System.EnterpriseServices.Crm.ApplicationCrmEnabledAttribute")]
[AttributeUsage(AttributeTargets.Assembly)]
[ComVisible(false)]
public sealed class ApplicationCrmEnabledAttribute : Attribute
{
	private bool val;

	public bool Value => val;

	public ApplicationCrmEnabledAttribute()
	{
		val = true;
	}

	public ApplicationCrmEnabledAttribute(bool val)
	{
		this.val = val;
	}
}
