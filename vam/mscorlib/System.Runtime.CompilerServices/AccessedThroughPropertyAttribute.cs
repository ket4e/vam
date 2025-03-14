using System.Runtime.InteropServices;

namespace System.Runtime.CompilerServices;

[ComVisible(true)]
[AttributeUsage(AttributeTargets.Field)]
public sealed class AccessedThroughPropertyAttribute : Attribute
{
	private string name;

	public string PropertyName => name;

	public AccessedThroughPropertyAttribute(string propertyName)
	{
		name = propertyName;
	}
}
