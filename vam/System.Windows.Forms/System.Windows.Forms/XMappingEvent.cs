namespace System.Windows.Forms;

internal struct XMappingEvent
{
	internal XEventName type;

	internal IntPtr serial;

	internal bool send_event;

	internal IntPtr display;

	internal IntPtr window;

	internal int request;

	internal int first_keycode;

	internal int count;
}
