using System.Security.Permissions;

namespace System.Drawing;

public sealed class BufferedGraphics : IDisposable
{
	private Rectangle size;

	private Bitmap membmp;

	private Graphics target;

	private Graphics source;

	public Graphics Graphics
	{
		get
		{
			if (source == null)
			{
				source = Graphics.FromImage(membmp);
			}
			return source;
		}
	}

	private BufferedGraphics()
	{
	}

	internal BufferedGraphics(Graphics targetGraphics, Rectangle targetRectangle)
	{
		size = targetRectangle;
		target = targetGraphics;
		membmp = new Bitmap(size.Width, size.Height);
	}

	~BufferedGraphics()
	{
		Dispose(disposing: false);
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	private void Dispose(bool disposing)
	{
		if (disposing)
		{
			if (membmp != null)
			{
				membmp.Dispose();
				membmp = null;
			}
			if (source != null)
			{
				source.Dispose();
				source = null;
			}
			target = null;
		}
	}

	public void Render()
	{
		Render(target);
	}

	public void Render(Graphics target)
	{
		target?.DrawImage(membmp, size);
	}

	[System.MonoTODO("The targetDC parameter has no equivalent in libgdiplus.")]
	[PermissionSet(SecurityAction.Demand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\">\n<IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\nversion=\"1\"\nFlags=\"UnmanagedCode\"/>\n</PermissionSet>\n")]
	public void Render(IntPtr targetDC)
	{
		throw new NotImplementedException();
	}
}
