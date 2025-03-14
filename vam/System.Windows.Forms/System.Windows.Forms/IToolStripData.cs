namespace System.Windows.Forms;

internal interface IToolStripData
{
	bool IsCurrentlyDragging { get; }

	bool Stretch { get; set; }
}
