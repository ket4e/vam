using System;
using System.Security.Permissions;

namespace Microsoft.Win32;

[PermissionSet((SecurityAction)15, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\"\nUnrestricted=\"true\"/>\n")]
[PermissionSet((SecurityAction)14, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\"\nUnrestricted=\"true\"/>\n")]
public class SessionSwitchEventArgs : EventArgs
{
	private SessionSwitchReason reason;

	public SessionSwitchReason Reason => reason;

	public SessionSwitchEventArgs(SessionSwitchReason reason)
	{
		this.reason = reason;
	}
}
