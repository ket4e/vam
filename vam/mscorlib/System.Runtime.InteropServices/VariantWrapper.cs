namespace System.Runtime.InteropServices;

[Serializable]
public sealed class VariantWrapper
{
	private object _wrappedObject;

	public object WrappedObject => _wrappedObject;

	public VariantWrapper(object obj)
	{
		_wrappedObject = obj;
	}
}
