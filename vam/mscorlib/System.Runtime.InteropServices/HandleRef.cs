namespace System.Runtime.InteropServices;

[ComVisible(true)]
public struct HandleRef
{
	private object wrapper;

	private IntPtr handle;

	public IntPtr Handle => handle;

	public object Wrapper => wrapper;

	public HandleRef(object wrapper, IntPtr handle)
	{
		this.wrapper = wrapper;
		this.handle = handle;
	}

	public static IntPtr ToIntPtr(HandleRef value)
	{
		return value.Handle;
	}

	public static explicit operator IntPtr(HandleRef value)
	{
		return value.Handle;
	}
}
