namespace System.Windows.Forms;

internal struct XNoExposeEvent
{
	internal XEventName type;

	internal IntPtr serial;

	internal bool send_event;

	internal IntPtr display;

	internal IntPtr drawable;

	internal int major_code;

	internal int minor_code;
}
