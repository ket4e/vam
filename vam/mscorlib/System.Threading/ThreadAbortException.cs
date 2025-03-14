using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System.Threading;

[Serializable]
[ComVisible(true)]
public sealed class ThreadAbortException : SystemException
{
	public object ExceptionState => Thread.CurrentThread.GetAbortExceptionState();

	private ThreadAbortException()
		: base("Thread was being aborted")
	{
		base.HResult = -2146233040;
	}

	private ThreadAbortException(SerializationInfo info, StreamingContext sc)
		: base(info, sc)
	{
	}
}
