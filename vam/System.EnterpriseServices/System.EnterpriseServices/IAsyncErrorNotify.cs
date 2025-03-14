using System.Runtime.InteropServices;

namespace System.EnterpriseServices;

[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid("FE6777FB-A674-4177-8F32-6D707E113484")]
public interface IAsyncErrorNotify
{
	void OnError(int hresult);
}
