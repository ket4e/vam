using System.Runtime.InteropServices;

namespace System.Reflection;

[ComVisible(true)]
[AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
public sealed class AssemblyTrademarkAttribute : Attribute
{
	private string name;

	public string Trademark => name;

	public AssemblyTrademarkAttribute(string trademark)
	{
		name = trademark;
	}
}
