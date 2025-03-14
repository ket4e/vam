using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;

namespace Un4seen.Bass.Misc;

[SuppressUnmanagedCodeSecurity]
public sealed class HiPerfTimer
{
	private long startTime;

	private long stopTime;

	private long freq;

	public double Duration => (double)(stopTime - startTime) / (double)freq;

	[DllImport("Kernel32.dll")]
	private static extern bool QueryPerformanceCounter(out long lpPerformanceCount);

	[DllImport("Kernel32.dll")]
	private static extern bool QueryPerformanceFrequency(out long lpFrequency);

	public HiPerfTimer()
	{
		startTime = 0L;
		stopTime = 0L;
		if (!QueryPerformanceFrequency(out freq))
		{
			throw new Win32Exception();
		}
	}

	public void Start()
	{
		Thread.Sleep(0);
		QueryPerformanceCounter(out startTime);
	}

	public void Stop()
	{
		QueryPerformanceCounter(out stopTime);
	}
}
