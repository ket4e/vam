namespace System.Windows.Forms;

internal struct XMapRequestEvent
{
	internal XEventName type;

	internal IntPtr serial;

	internal bool send_event;

	internal IntPtr display;

	internal IntPtr parent;

	internal IntPtr window;
}
