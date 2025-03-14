using System.Runtime.InteropServices;

namespace System.Reflection;

[ComVisible(true)]
[AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
public sealed class AssemblyDefaultAliasAttribute : Attribute
{
	private string name;

	public string DefaultAlias => name;

	public AssemblyDefaultAliasAttribute(string defaultAlias)
	{
		name = defaultAlias;
	}
}
