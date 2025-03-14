namespace System.Windows.Forms;

[Flags]
internal enum MotifInputMode
{
	Modeless = 0,
	ApplicationModal = 1,
	SystemModal = 2,
	FullApplicationModal = 3
}
