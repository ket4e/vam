namespace System.Windows.Forms;

internal struct XButtonEvent
{
	internal XEventName type;

	internal IntPtr serial;

	internal bool send_event;

	internal IntPtr display;

	internal IntPtr window;

	internal IntPtr root;

	internal IntPtr subwindow;

	internal IntPtr time;

	internal int x;

	internal int y;

	internal int x_root;

	internal int y_root;

	internal int state;

	internal int button;

	internal bool same_screen;
}
