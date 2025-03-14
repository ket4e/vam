namespace System.Windows.Forms;

internal struct XSelectionEvent
{
	internal XEventName type;

	internal IntPtr serial;

	internal bool send_event;

	internal IntPtr display;

	internal IntPtr requestor;

	internal IntPtr selection;

	internal IntPtr target;

	internal IntPtr property;

	internal IntPtr time;
}
