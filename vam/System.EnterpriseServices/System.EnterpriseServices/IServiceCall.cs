using System.Runtime.InteropServices;

namespace System.EnterpriseServices;

[ComImport]
[Guid("BD3E2E12-42DD-40f4-A09A-95A50C58304B")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface IServiceCall
{
	void OnCall();
}
