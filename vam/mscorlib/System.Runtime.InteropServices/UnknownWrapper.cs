namespace System.Runtime.InteropServices;

[Serializable]
[ComVisible(true)]
public sealed class UnknownWrapper
{
	private object InternalObject;

	public object WrappedObject => InternalObject;

	public UnknownWrapper(object obj)
	{
		InternalObject = obj;
	}
}
