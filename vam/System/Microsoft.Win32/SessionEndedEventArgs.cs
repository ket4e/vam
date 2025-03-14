using System;
using System.Security.Permissions;

namespace Microsoft.Win32;

[PermissionSet((SecurityAction)15, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\"\nUnrestricted=\"true\"/>\n")]
[PermissionSet((SecurityAction)14, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\"\nUnrestricted=\"true\"/>\n")]
public class SessionEndedEventArgs : EventArgs
{
	private SessionEndReasons myreason;

	public SessionEndReasons Reason => myreason;

	public SessionEndedEventArgs(SessionEndReasons reason)
	{
		myreason = reason;
	}
}
