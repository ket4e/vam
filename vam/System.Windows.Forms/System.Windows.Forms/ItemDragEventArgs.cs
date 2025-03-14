using System.Runtime.InteropServices;

namespace System.Windows.Forms;

[ComVisible(true)]
public class ItemDragEventArgs : EventArgs
{
	private MouseButtons button;

	private object item;

	public MouseButtons Button => button;

	public object Item => item;

	public ItemDragEventArgs(MouseButtons button)
	{
		this.button = button;
	}

	public ItemDragEventArgs(MouseButtons button, object item)
	{
		this.button = button;
		this.item = item;
	}
}
