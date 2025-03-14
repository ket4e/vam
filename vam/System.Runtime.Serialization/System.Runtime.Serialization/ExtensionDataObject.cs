namespace System.Runtime.Serialization;

public sealed class ExtensionDataObject
{
	private object target;

	internal ExtensionDataObject(object target)
	{
		this.target = target;
	}
}
