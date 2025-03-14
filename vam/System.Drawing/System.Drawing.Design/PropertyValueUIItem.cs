using System.Security.Permissions;

namespace System.Drawing.Design;

[PermissionSet((SecurityAction)14, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\"\nUnrestricted=\"true\"/>\n")]
[PermissionSet((SecurityAction)15, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\"\nUnrestricted=\"true\"/>\n")]
public class PropertyValueUIItem
{
	private Image uiItemImage;

	private PropertyValueUIItemInvokeHandler handler;

	private string tooltip;

	public virtual Image Image => uiItemImage;

	public virtual PropertyValueUIItemInvokeHandler InvokeHandler => handler;

	public virtual string ToolTip => tooltip;

	public PropertyValueUIItem(Image uiItemImage, PropertyValueUIItemInvokeHandler handler, string tooltip)
	{
		if (uiItemImage == null)
		{
			throw new ArgumentNullException("uiItemImage");
		}
		if (handler == null)
		{
			throw new ArgumentNullException("handler");
		}
		this.uiItemImage = uiItemImage;
		this.handler = handler;
		this.tooltip = tooltip;
	}

	public virtual void Reset()
	{
	}
}
