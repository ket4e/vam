namespace System.Runtime.InteropServices;

[ComVisible(true)]
[AttributeUsage(AttributeTargets.Interface, Inherited = false)]
public sealed class TypeLibImportClassAttribute : Attribute
{
	private string _importClass;

	public string Value => _importClass;

	public TypeLibImportClassAttribute(Type importClass)
	{
		_importClass = importClass.ToString();
	}
}
