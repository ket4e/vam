using System.ComponentModel;
using System.Security.Permissions;

namespace System.Drawing.Design;

[PermissionSet((SecurityAction)14, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\"\nUnrestricted=\"true\"/>\n")]
[PermissionSet((SecurityAction)15, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\"\nUnrestricted=\"true\"/>\n")]
public class PaintValueEventArgs : EventArgs
{
	private ITypeDescriptorContext context;

	private object value;

	private Graphics graphics;

	private Rectangle bounds;

	public Rectangle Bounds => bounds;

	public ITypeDescriptorContext Context => context;

	public Graphics Graphics => graphics;

	public object Value => value;

	public PaintValueEventArgs(ITypeDescriptorContext context, object value, Graphics graphics, Rectangle bounds)
	{
		if (graphics == null)
		{
			throw new ArgumentNullException("graphics");
		}
		this.context = context;
		this.value = value;
		this.graphics = graphics;
		this.bounds = bounds;
	}
}
