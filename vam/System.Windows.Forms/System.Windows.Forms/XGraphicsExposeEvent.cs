namespace System.Windows.Forms;

internal struct XGraphicsExposeEvent
{
	internal XEventName type;

	internal IntPtr serial;

	internal bool send_event;

	internal IntPtr display;

	internal IntPtr drawable;

	internal int x;

	internal int y;

	internal int width;

	internal int height;

	internal int count;

	internal int major_code;

	internal int minor_code;
}
