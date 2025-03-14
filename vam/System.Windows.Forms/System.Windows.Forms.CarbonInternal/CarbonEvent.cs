namespace System.Windows.Forms.CarbonInternal;

internal struct CarbonEvent
{
	public IntPtr hWnd;

	public IntPtr evt;

	public CarbonEvent(IntPtr hWnd, IntPtr evt)
	{
		this.hWnd = hWnd;
		this.evt = evt;
	}
}
