namespace System.Windows.Forms;

internal struct XConfigureEvent
{
	internal XEventName type;

	internal IntPtr serial;

	internal bool send_event;

	internal IntPtr display;

	internal IntPtr xevent;

	internal IntPtr window;

	internal int x;

	internal int y;

	internal int width;

	internal int height;

	internal int border_width;

	internal IntPtr above;

	internal bool override_redirect;
}
