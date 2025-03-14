using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Mono.Mozilla;

internal class Stream : nsIOutputStream, nsIInputStream
{
	private System.IO.Stream back;

	public System.IO.Stream BaseStream => back;

	public Stream(System.IO.Stream stream)
	{
		back = stream;
	}

	public int close()
	{
		back.Close();
		return 0;
	}

	public int flush()
	{
		back.Flush();
		return 0;
	}

	public int write([MarshalAs(UnmanagedType.LPStr)] string str, uint count, out uint ret)
	{
		ret = count;
		if (count == 0)
		{
			return 0;
		}
		byte[] bytes = Encoding.ASCII.GetBytes(str);
		back.Write(bytes, 0, (int)count);
		return 0;
	}

	public int writeFrom([MarshalAs(UnmanagedType.Interface)] nsIInputStream aFromStream, uint aCount, out uint ret)
	{
		ret = 0u;
		return 0;
	}

	public int writeSegments(nsIReadSegmentFunDelegate aReader, IntPtr aClosure, uint aCount, out uint ret)
	{
		ret = 0u;
		return 0;
	}

	public int isNonBlocking(out bool ret)
	{
		ret = false;
		return 0;
	}

	public int available(out uint ret)
	{
		ret = 0u;
		return 0;
	}

	public int read(HandleRef str, uint count, out uint ret)
	{
		byte[] array = new byte[count];
		ret = (uint)back.Read(array, 0, (int)count);
		string @string = Encoding.ASCII.GetString(array);
		Base.StringSet(str, @string);
		return 0;
	}

	public int readSegments(nsIWriteSegmentFunDelegate aWriter, IntPtr aClosure, uint aCount, out uint ret)
	{
		ret = 0u;
		return 0;
	}
}
