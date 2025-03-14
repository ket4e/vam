using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32.SafeHandles;

namespace System.IO.Pipes;

internal abstract class Win32NamedPipe : IPipe
{
	private string name_cache;

	public string Name
	{
		get
		{
			if (name_cache != null)
			{
				return name_cache;
			}
			byte[] array = new byte[200];
			while (true)
			{
				if (!Win32Marshal.GetNamedPipeHandleState(Handle, out var _, out var _, out var _, out var _, array, array.Length))
				{
					int lastWin32Error = Marshal.GetLastWin32Error();
					throw new Win32Exception(lastWin32Error);
				}
				if (array[array.Length - 1] == 0)
				{
					break;
				}
				array = new byte[array.Length * 10];
			}
			name_cache = Encoding.Default.GetString(array);
			return name_cache;
		}
	}

	public abstract SafePipeHandle Handle { get; }

	public void WaitForPipeDrain()
	{
		throw new NotImplementedException();
	}
}
