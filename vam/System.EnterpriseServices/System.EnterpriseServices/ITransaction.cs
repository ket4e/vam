using System.Runtime.InteropServices;

namespace System.EnterpriseServices;

[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid("0FB15084-AF41-11CE-BD2B-204C4F4F5020")]
public interface ITransaction
{
	void Abort(ref BOID pboidReason, int fRetaining, int fAsync);

	void Commit(int fRetaining, int grfTC, int grfRM);

	void GetTransactionInfo(out XACTTRANSINFO pinfo);
}
