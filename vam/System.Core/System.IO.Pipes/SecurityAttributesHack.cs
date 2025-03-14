namespace System.IO.Pipes;

internal struct SecurityAttributesHack
{
	public readonly int Length;

	public readonly IntPtr SecurityDescriptor;

	public readonly bool Inheritable;

	public SecurityAttributesHack(bool inheritable)
	{
		Length = 0;
		SecurityDescriptor = IntPtr.Zero;
		Inheritable = inheritable;
	}
}
