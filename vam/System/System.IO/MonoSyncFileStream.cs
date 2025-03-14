using System.Runtime.Remoting.Messaging;

namespace System.IO;

internal class MonoSyncFileStream : FileStream
{
	private delegate void WriteDelegate(byte[] buffer, int offset, int count);

	private delegate int ReadDelegate(byte[] buffer, int offset, int count);

	public MonoSyncFileStream(IntPtr handle, FileAccess access, bool ownsHandle, int bufferSize)
		: base(handle, access, ownsHandle, bufferSize, isAsync: false)
	{
	}

	public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback cback, object state)
	{
		if (!CanWrite)
		{
			throw new NotSupportedException("This stream does not support writing");
		}
		if (buffer == null)
		{
			throw new ArgumentNullException("buffer");
		}
		if (count < 0)
		{
			throw new ArgumentOutOfRangeException("count", "Must be >= 0");
		}
		if (offset < 0)
		{
			throw new ArgumentOutOfRangeException("offset", "Must be >= 0");
		}
		WriteDelegate writeDelegate = Write;
		return writeDelegate.BeginInvoke(buffer, offset, count, cback, state);
	}

	public override void EndWrite(IAsyncResult asyncResult)
	{
		if (asyncResult == null)
		{
			throw new ArgumentNullException("asyncResult");
		}
		if (!(asyncResult is AsyncResult asyncResult2))
		{
			throw new ArgumentException("Invalid IAsyncResult", "asyncResult");
		}
		if (!(asyncResult2.AsyncDelegate is WriteDelegate writeDelegate))
		{
			throw new ArgumentException("Invalid IAsyncResult", "asyncResult");
		}
		writeDelegate.EndInvoke(asyncResult);
	}

	public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback cback, object state)
	{
		if (!CanRead)
		{
			throw new NotSupportedException("This stream does not support reading");
		}
		if (buffer == null)
		{
			throw new ArgumentNullException("buffer");
		}
		if (count < 0)
		{
			throw new ArgumentOutOfRangeException("count", "Must be >= 0");
		}
		if (offset < 0)
		{
			throw new ArgumentOutOfRangeException("offset", "Must be >= 0");
		}
		ReadDelegate readDelegate = Read;
		return readDelegate.BeginInvoke(buffer, offset, count, cback, state);
	}

	public override int EndRead(IAsyncResult asyncResult)
	{
		if (asyncResult == null)
		{
			throw new ArgumentNullException("asyncResult");
		}
		if (!(asyncResult is AsyncResult asyncResult2))
		{
			throw new ArgumentException("Invalid IAsyncResult", "asyncResult");
		}
		if (!(asyncResult2.AsyncDelegate is ReadDelegate readDelegate))
		{
			throw new ArgumentException("Invalid IAsyncResult", "asyncResult");
		}
		return readDelegate.EndInvoke(asyncResult);
	}
}
