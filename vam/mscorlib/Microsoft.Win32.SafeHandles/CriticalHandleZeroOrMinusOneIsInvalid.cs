using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;

namespace Microsoft.Win32.SafeHandles;

public abstract class CriticalHandleZeroOrMinusOneIsInvalid : CriticalHandle, IDisposable
{
	public override bool IsInvalid => handle == (IntPtr)(-1) || handle == IntPtr.Zero;

	[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
	protected CriticalHandleZeroOrMinusOneIsInvalid()
		: base((IntPtr)(-1))
	{
	}
}
