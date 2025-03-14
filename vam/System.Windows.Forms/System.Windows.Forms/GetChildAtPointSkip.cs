namespace System.Windows.Forms;

[Flags]
public enum GetChildAtPointSkip
{
	None = 0,
	Invisible = 1,
	Disabled = 2,
	Transparent = 4
}
