using System.Runtime.InteropServices;

namespace System.EnterpriseServices;

[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid("72380d55-8d2b-43a3-8513-2b6ef31434e9")]
public interface IProcessInitControl
{
	void ResetInitializerTimeout(int dwSecondsRemaining);
}
