namespace System.Windows.Forms.CarbonInternal;

internal interface IEventHandler
{
	bool ProcessEvent(IntPtr callref, IntPtr eventref, IntPtr handle, uint kind, ref MSG msg);
}
