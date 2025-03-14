namespace System.CodeDom.Compiler;

[AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
public sealed class GeneratedCodeAttribute : Attribute
{
	private string tool;

	private string version;

	public string Tool => tool;

	public string Version => version;

	public GeneratedCodeAttribute(string tool, string version)
	{
		this.tool = tool;
		this.version = version;
	}
}
