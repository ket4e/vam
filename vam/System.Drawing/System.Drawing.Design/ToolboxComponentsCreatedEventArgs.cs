using System.ComponentModel;
using System.Security.Permissions;

namespace System.Drawing.Design;

[PermissionSet((SecurityAction)14, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\"\nUnrestricted=\"true\"/>\n")]
[PermissionSet((SecurityAction)15, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\"\nUnrestricted=\"true\"/>\n")]
public class ToolboxComponentsCreatedEventArgs : EventArgs
{
	private IComponent[] components;

	public IComponent[] Components => components;

	public ToolboxComponentsCreatedEventArgs(IComponent[] components)
	{
		this.components = components;
	}
}
