using System.Runtime.InteropServices;

namespace System.Reflection;

[AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
[ComVisible(true)]
public sealed class AssemblyVersionAttribute : Attribute
{
	private string name;

	public string Version => name;

	public AssemblyVersionAttribute(string version)
	{
		name = version;
	}
}
