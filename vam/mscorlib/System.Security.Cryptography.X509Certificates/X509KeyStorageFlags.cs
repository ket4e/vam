using System.Runtime.InteropServices;

namespace System.Security.Cryptography.X509Certificates;

[Serializable]
[Flags]
[ComVisible(true)]
public enum X509KeyStorageFlags
{
	DefaultKeySet = 0,
	UserKeySet = 1,
	MachineKeySet = 2,
	Exportable = 4,
	UserProtected = 8,
	PersistKeySet = 0x10
}
