using System.Runtime.InteropServices;
using System.Threading;

namespace System.Windows.Forms;

internal class XplatUIDriverSupport
{
	internal static void ExecutionCallback(object state)
	{
		AsyncMethodData asyncMethodData = (AsyncMethodData)state;
		AsyncMethodResult result = asyncMethodData.Result;
		object result2;
		try
		{
			result2 = asyncMethodData.Method.DynamicInvoke(asyncMethodData.Args);
		}
		catch (Exception ex)
		{
			if (result != null)
			{
				result.CompleteWithException(ex);
				return;
			}
			throw;
		}
		result?.Complete(result2);
	}

	internal static void ExecuteClientMessage(GCHandle gchandle)
	{
		AsyncMethodData asyncMethodData = (AsyncMethodData)gchandle.Target;
		try
		{
			if (asyncMethodData.Context == null)
			{
				ExecutionCallback(asyncMethodData);
			}
			else
			{
				ExecutionContext.Run(asyncMethodData.Context, ExecutionCallback, asyncMethodData);
			}
		}
		finally
		{
			gchandle.Free();
		}
	}
}
