using System.Drawing;

namespace System.Windows.Forms;

public class ToolStripItemImageRenderEventArgs : ToolStripItemRenderEventArgs
{
	private Image image;

	private Rectangle image_rectangle;

	public Image Image => image;

	public Rectangle ImageRectangle => image_rectangle;

	public ToolStripItemImageRenderEventArgs(Graphics g, ToolStripItem item, Rectangle imageRectangle)
		: this(g, item, null, imageRectangle)
	{
	}

	public ToolStripItemImageRenderEventArgs(Graphics g, ToolStripItem item, Image image, Rectangle imageRectangle)
		: base(g, item)
	{
		this.image = image;
		image_rectangle = imageRectangle;
	}
}
