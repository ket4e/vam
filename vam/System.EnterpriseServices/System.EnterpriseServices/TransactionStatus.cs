using System.Runtime.InteropServices;

namespace System.EnterpriseServices;

[Serializable]
[ComVisible(false)]
public enum TransactionStatus
{
	Commited,
	LocallyOk,
	NoTransaction,
	Aborting,
	Aborted
}
