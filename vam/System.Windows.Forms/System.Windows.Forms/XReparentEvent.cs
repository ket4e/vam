namespace System.Windows.Forms;

internal struct XReparentEvent
{
	internal XEventName type;

	internal IntPtr serial;

	internal bool send_event;

	internal IntPtr display;

	internal IntPtr xevent;

	internal IntPtr window;

	internal IntPtr parent;

	internal int x;

	internal int y;

	internal bool override_redirect;
}
