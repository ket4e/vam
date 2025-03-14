using System.Runtime.InteropServices;

namespace System.IO.Ports;

[StructLayout(LayoutKind.Sequential)]
internal class Timeouts
{
	public const uint MaxDWord = uint.MaxValue;

	public uint ReadIntervalTimeout;

	public uint ReadTotalTimeoutMultiplier;

	public uint ReadTotalTimeoutConstant;

	public uint WriteTotalTimeoutMultiplier;

	public uint WriteTotalTimeoutConstant;

	public Timeouts(int read_timeout, int write_timeout)
	{
		SetValues(read_timeout, write_timeout);
	}

	public void SetValues(int read_timeout, int write_timeout)
	{
		ReadIntervalTimeout = uint.MaxValue;
		ReadTotalTimeoutMultiplier = uint.MaxValue;
		ReadTotalTimeoutConstant = ((read_timeout != -1) ? ((uint)read_timeout) : 4294967294u);
		WriteTotalTimeoutMultiplier = 0u;
		WriteTotalTimeoutConstant = ((write_timeout != -1) ? ((uint)write_timeout) : uint.MaxValue);
	}
}
