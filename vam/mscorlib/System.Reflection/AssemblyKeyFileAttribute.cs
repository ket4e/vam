using System.Runtime.InteropServices;

namespace System.Reflection;

[AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
[ComVisible(true)]
public sealed class AssemblyKeyFileAttribute : Attribute
{
	private string name;

	public string KeyFile => name;

	public AssemblyKeyFileAttribute(string keyFile)
	{
		name = keyFile;
	}
}
