using System.Runtime.InteropServices;

namespace System.EnterpriseServices;

[ComVisible(false)]
public sealed class SharedProperty
{
	private ISharedProperty property;

	public object Value
	{
		get
		{
			return property.Value;
		}
		set
		{
			property.Value = value;
		}
	}

	internal SharedProperty(ISharedProperty property)
	{
		this.property = property;
	}
}
