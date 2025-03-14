using System.Threading;

namespace System.Windows.Forms;

internal class AsyncMethodData
{
	public IntPtr Handle;

	public Delegate Method;

	public object[] Args;

	public AsyncMethodResult Result;

	public ExecutionContext Context;
}
