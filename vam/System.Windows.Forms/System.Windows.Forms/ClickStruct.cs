namespace System.Windows.Forms;

internal struct ClickStruct
{
	internal IntPtr Hwnd;

	internal Msg Message;

	internal IntPtr wParam;

	internal IntPtr lParam;

	internal long Time;

	internal bool Pending;
}
