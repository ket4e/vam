using System.Drawing;

namespace System.Windows.Forms;

public class PaintEventArgs : EventArgs, IDisposable
{
	private Graphics graphics;

	private Rectangle clip_rectangle;

	internal bool Handled;

	private bool disposed;

	public Rectangle ClipRectangle => clip_rectangle;

	public Graphics Graphics => graphics;

	public PaintEventArgs(Graphics graphics, Rectangle clipRect)
	{
		if (graphics == null)
		{
			throw new ArgumentNullException("graphics");
		}
		this.graphics = graphics;
		clip_rectangle = clipRect;
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	internal Graphics SetGraphics(Graphics g)
	{
		Graphics result = graphics;
		graphics = g;
		return result;
	}

	internal void SetClip(Rectangle clip)
	{
		clip_rectangle = clip;
	}

	~PaintEventArgs()
	{
		Dispose(disposing: false);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (!disposed)
		{
			disposed = true;
		}
	}
}
