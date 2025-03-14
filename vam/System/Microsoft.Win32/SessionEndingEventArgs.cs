using System;
using System.Security.Permissions;

namespace Microsoft.Win32;

[PermissionSet((SecurityAction)15, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\"\nUnrestricted=\"true\"/>\n")]
[PermissionSet((SecurityAction)14, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\"\nUnrestricted=\"true\"/>\n")]
public class SessionEndingEventArgs : EventArgs
{
	private SessionEndReasons myreason;

	private bool mycancel;

	public SessionEndReasons Reason => myreason;

	public bool Cancel
	{
		get
		{
			return mycancel;
		}
		set
		{
			mycancel = value;
		}
	}

	public SessionEndingEventArgs(SessionEndReasons reason)
	{
		myreason = reason;
	}
}
