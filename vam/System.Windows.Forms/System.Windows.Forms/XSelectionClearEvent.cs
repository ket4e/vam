namespace System.Windows.Forms;

internal struct XSelectionClearEvent
{
	internal XEventName type;

	internal IntPtr serial;

	internal bool send_event;

	internal IntPtr display;

	internal IntPtr window;

	internal IntPtr selection;

	internal IntPtr time;
}
