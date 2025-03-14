namespace System.Windows.Forms.CarbonInternal;

internal struct Caret
{
	internal Timer Timer;

	internal IntPtr Hwnd;

	internal int X;

	internal int Y;

	internal int Width;

	internal int Height;

	internal int Visible;

	internal bool On;

	internal bool Paused;
}
