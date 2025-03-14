using System.Runtime.InteropServices;

namespace System.EnterpriseServices;

[ComVisible(false)]
[AttributeUsage(AttributeTargets.Class)]
public sealed class ExceptionClassAttribute : Attribute
{
	private string name;

	public string Value => name;

	public ExceptionClassAttribute(string name)
	{
		this.name = name;
	}
}
