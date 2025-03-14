using System.Runtime.InteropServices;

namespace System.Data.Odbc;

internal sealed class NativeBuffer : IDisposable
{
	private IntPtr _ptr;

	private int _length;

	private bool disposed;

	public IntPtr Handle
	{
		get
		{
			return _ptr;
		}
		set
		{
			_ptr = value;
		}
	}

	public int Size => _length;

	public void AllocBuffer(int length)
	{
		FreeBuffer();
		_ptr = Marshal.AllocCoTaskMem(length);
		_length = length;
	}

	public void FreeBuffer()
	{
		if (!(_ptr == IntPtr.Zero))
		{
			Marshal.FreeCoTaskMem(_ptr);
			_length = 0;
			_ptr = IntPtr.Zero;
		}
	}

	public void EnsureAlloc(int length)
	{
		if (Size != length || !(_ptr != IntPtr.Zero))
		{
			AllocBuffer(length);
		}
	}

	public void Dispose(bool disposing)
	{
		if (!disposed)
		{
			FreeBuffer();
			_ptr = IntPtr.Zero;
			disposed = true;
		}
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	~NativeBuffer()
	{
		Dispose(disposing: false);
	}

	public static implicit operator IntPtr(NativeBuffer buf)
	{
		return buf.Handle;
	}
}
