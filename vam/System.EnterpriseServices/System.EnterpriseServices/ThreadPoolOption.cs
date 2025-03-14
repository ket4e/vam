using System.Runtime.InteropServices;

namespace System.EnterpriseServices;

[Serializable]
[ComVisible(false)]
public enum ThreadPoolOption
{
	None,
	Inherit,
	STA,
	MTA
}
