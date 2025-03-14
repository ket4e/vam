using System.ComponentModel.Design;
using System.Security.Permissions;

namespace System.Drawing.Design;

[PermissionSet((SecurityAction)15, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\"\nUnrestricted=\"true\"/>\n")]
[PermissionSet((SecurityAction)14, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\"\nUnrestricted=\"true\"/>\n")]
public class ToolboxComponentsCreatingEventArgs : EventArgs
{
	private IDesignerHost host;

	public IDesignerHost DesignerHost => host;

	public ToolboxComponentsCreatingEventArgs(IDesignerHost host)
	{
		this.host = host;
	}
}
