using System.Runtime.InteropServices;

namespace System.Windows.Forms.CarbonInternal;

internal class HIObjectHandler : EventHandlerBase, IEventHandler
{
	internal const uint kEventHIObjectConstruct = 1u;

	internal const uint kEventHIObjectInitialize = 2u;

	internal const uint kEventHIObjectDestruct = 3u;

	internal HIObjectHandler(XplatUICarbon driver)
		: base(driver)
	{
	}

	public bool ProcessEvent(IntPtr callref, IntPtr eventref, IntPtr handle, uint kind, ref MSG msg)
	{
		switch (kind)
		{
		case 1u:
		{
			IntPtr data = IntPtr.Zero;
			GetEventParameter(eventref, 1751740265u, 1751740258u, IntPtr.Zero, 4u, IntPtr.Zero, ref data);
			return false;
		}
		case 2u:
			CallNextEventHandler(callref, eventref);
			return false;
		case 3u:
			return false;
		default:
			return false;
		}
	}

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	private static extern int CallNextEventHandler(IntPtr callref, IntPtr eventref);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	private static extern int GetEventParameter(IntPtr eventref, uint name, uint type, IntPtr outtype, uint size, IntPtr outsize, ref IntPtr data);
}
