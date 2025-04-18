using System.Security.Permissions;

namespace System.Drawing;

public sealed class BufferedGraphicsContext : IDisposable
{
	private Size max_buffer;

	public Size MaximumBuffer
	{
		get
		{
			return max_buffer;
		}
		set
		{
			if (value.Width <= 0 || value.Height <= 0)
			{
				throw new ArgumentException("The height or width of the size is less than or equal to zero.");
			}
			max_buffer = value;
		}
	}

	public BufferedGraphicsContext()
	{
		max_buffer = Size.Empty;
	}

	~BufferedGraphicsContext()
	{
	}

	public BufferedGraphics Allocate(Graphics targetGraphics, Rectangle targetRectangle)
	{
		return new BufferedGraphics(targetGraphics, targetRectangle);
	}

	[System.MonoTODO("The targetDC parameter has no equivalent in libgdiplus.")]
	[PermissionSet(SecurityAction.Demand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\">\n<IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\nversion=\"1\"\nFlags=\"UnmanagedCode\"/>\n</PermissionSet>\n")]
	public BufferedGraphics Allocate(IntPtr targetDC, Rectangle targetRectangle)
	{
		throw new NotImplementedException();
	}

	public void Dispose()
	{
		GC.SuppressFinalize(this);
	}

	public void Invalidate()
	{
	}
}
