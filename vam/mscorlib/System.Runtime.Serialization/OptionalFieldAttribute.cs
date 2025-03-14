using System.Runtime.InteropServices;

namespace System.Runtime.Serialization;

[ComVisible(true)]
[AttributeUsage(AttributeTargets.Field, Inherited = false)]
public sealed class OptionalFieldAttribute : Attribute
{
	private int version_added;

	public int VersionAdded
	{
		get
		{
			return version_added;
		}
		set
		{
			version_added = value;
		}
	}
}
