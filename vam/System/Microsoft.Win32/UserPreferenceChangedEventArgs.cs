using System;
using System.Security.Permissions;

namespace Microsoft.Win32;

[PermissionSet((SecurityAction)14, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\"\nUnrestricted=\"true\"/>\n")]
[PermissionSet((SecurityAction)15, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\"\nUnrestricted=\"true\"/>\n")]
public class UserPreferenceChangedEventArgs : EventArgs
{
	private UserPreferenceCategory mycategory;

	public UserPreferenceCategory Category => mycategory;

	public UserPreferenceChangedEventArgs(UserPreferenceCategory category)
	{
		mycategory = category;
	}
}
