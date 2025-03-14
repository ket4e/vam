namespace System.Windows.Forms;

[Flags]
public enum MessageBoxOptions
{
	DefaultDesktopOnly = 0x20000,
	RightAlign = 0x80000,
	RtlReading = 0x100000,
	ServiceNotification = 0x200000
}
