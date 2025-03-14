namespace System.Windows.Forms.Theming;

[Flags]
internal enum ButtonThemeState
{
	Normal = 1,
	Entered = 2,
	Pressed = 4,
	Disabled = 8,
	Default = 0x10
}
