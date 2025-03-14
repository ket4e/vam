using System;
using System.Runtime.InteropServices;

namespace Mono.Mozilla;

internal class AsciiString : IDisposable
{
	[StructLayout(LayoutKind.Sequential)]
	private class nsStringContainer
	{
		private IntPtr v;

		private IntPtr d1;

		private uint d2;

		private IntPtr d3;
	}

	private bool disposed;

	private nsStringContainer unmanagedContainer;

	private HandleRef handle;

	private string str = string.Empty;

	private bool dirty;

	public HandleRef Handle
	{
		get
		{
			dirty = true;
			return handle;
		}
	}

	public string String
	{
		get
		{
			if (dirty)
			{
				Base.gluezilla_CStringGetData(handle, out var aBuf, out var _);
				str = Marshal.PtrToStringAnsi(aBuf);
				dirty = false;
			}
			return str;
		}
		set
		{
			if (str != value)
			{
				str = value;
				Base.gluezilla_CStringSetData(handle, str, (uint)str.Length);
			}
		}
	}

	public int Length => String.Length;

	public AsciiString(string value)
	{
		unmanagedContainer = new nsStringContainer();
		IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(nsStringContainer)));
		Marshal.StructureToPtr(unmanagedContainer, ptr, fDeleteOld: false);
		handle = new HandleRef(typeof(nsStringContainer), ptr);
		uint num = Base.gluezilla_CStringContainerInit(handle);
		String = value;
	}

	~AsciiString()
	{
		Dispose(disposing: false);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (!disposed)
		{
			if (disposing)
			{
				Base.gluezilla_CStringContainerFinish(handle);
				Marshal.FreeHGlobal(handle.Handle);
			}
			disposed = true;
		}
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	public override string ToString()
	{
		return String;
	}
}
