using System.Runtime.InteropServices;

namespace System.Security.AccessControl;

[ComVisible(false)]
[Flags]
public enum SemaphoreRights
{
	Modify = 2,
	Delete = 0x10000,
	ReadPermissions = 0x20000,
	ChangePermissions = 0x40000,
	TakeOwnership = 0x80000,
	Synchronize = 0x100000,
	FullControl = 0x1F0003
}
