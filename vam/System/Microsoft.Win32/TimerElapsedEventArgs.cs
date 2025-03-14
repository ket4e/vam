using System;
using System.Security.Permissions;

namespace Microsoft.Win32;

[PermissionSet((SecurityAction)15, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\"\nUnrestricted=\"true\"/>\n")]
[PermissionSet((SecurityAction)14, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\"\nUnrestricted=\"true\"/>\n")]
public class TimerElapsedEventArgs : EventArgs
{
	private IntPtr mytimerId;

	public IntPtr TimerId => mytimerId;

	public TimerElapsedEventArgs(IntPtr timerId)
	{
		mytimerId = timerId;
	}
}
