using System.Runtime.InteropServices;

namespace System.Reflection;

[AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
[ComVisible(true)]
public sealed class AssemblyKeyNameAttribute : Attribute
{
	private string name;

	public string KeyName => name;

	public AssemblyKeyNameAttribute(string keyName)
	{
		name = keyName;
	}
}
