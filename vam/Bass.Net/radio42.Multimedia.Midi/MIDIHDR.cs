using System;
using System.Runtime.InteropServices;
using System.Text;

namespace radio42.Multimedia.Midi;

[Serializable]
[StructLayout(LayoutKind.Sequential)]
internal class MIDIHDR
{
	public IntPtr data = IntPtr.Zero;

	public int bufferLength;

	public int bytesRecorded;

	public IntPtr user = IntPtr.Zero;

	public MIDIHeader flags;

	public IntPtr next = IntPtr.Zero;

	public IntPtr reserved = IntPtr.Zero;

	public int offset;

	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8, ArraySubType = UnmanagedType.SysInt)]
	public IntPtr[] reservedArray = new IntPtr[8];

	public bool IsDone => (flags & MIDIHeader.MHDR_DONE) != 0;

	public bool IsPrepared => (flags & MIDIHeader.MHDR_PREPARED) != 0;

	public bool IsStreamBuffer => (flags & MIDIHeader.MHDR_ISSTRM) != 0;

	public override string ToString()
	{
		if (data == IntPtr.Zero || !IsPrepared)
		{
			return null;
		}
		if (bytesRecorded <= 0)
		{
			return string.Empty;
		}
		StringBuilder stringBuilder = new StringBuilder(bytesRecorded);
		byte[] array = new byte[bytesRecorded];
		Marshal.Copy(data, array, 0, array.Length);
		byte[] array2 = array;
		foreach (byte b in array2)
		{
			stringBuilder.Append($"{b:X2} ");
		}
		return stringBuilder.ToString();
	}

	public byte[] GetData()
	{
		if (data == IntPtr.Zero || !IsPrepared)
		{
			return null;
		}
		if (bytesRecorded <= 0)
		{
			return new byte[0];
		}
		byte[] array = new byte[bytesRecorded];
		Marshal.Copy(data, array, 0, array.Length);
		return array;
	}

	public void Reset()
	{
		try
		{
			if (data != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(data);
			}
		}
		catch
		{
		}
		data = IntPtr.Zero;
		bufferLength = 0;
		bytesRecorded = 0;
		flags = MIDIHeader.MHDR_NONE;
		next = IntPtr.Zero;
		reserved = IntPtr.Zero;
		offset = 0;
		for (int i = 0; i < reservedArray.Length; i++)
		{
			reservedArray[i] = IntPtr.Zero;
		}
	}
}
