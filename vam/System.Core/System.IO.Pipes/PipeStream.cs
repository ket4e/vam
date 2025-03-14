using System.Security.Permissions;
using Microsoft.Win32.SafeHandles;

namespace System.IO.Pipes;

[PermissionSet((SecurityAction)15, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\"/>\n")]
[PermissionSet(SecurityAction.LinkDemand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\">\n<IPermission class=\"System.Security.Permissions.HostProtectionPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\nversion=\"1\"\nResources=\"None\"/>\n</PermissionSet>\n")]
public abstract class PipeStream : Stream
{
	internal const int DefaultBufferSize = 1024;

	private PipeDirection direction;

	private PipeTransmissionMode transmission_mode;

	private PipeTransmissionMode read_trans_mode;

	private int buffer_size;

	private SafePipeHandle handle;

	private Stream stream;

	private Func<byte[], int, int, int> read_delegate;

	private Action<byte[], int, int> write_delegate;

	internal static bool IsWindows => Win32Marshal.IsWindows;

	public override bool CanRead => (direction & PipeDirection.In) != 0;

	public override bool CanSeek => false;

	public override bool CanWrite => (direction & PipeDirection.Out) != 0;

	public virtual int InBufferSize => buffer_size;

	public bool IsAsync { get; private set; }

	public bool IsConnected { get; protected set; }

	internal Stream Stream
	{
		get
		{
			if (!IsConnected)
			{
				throw new InvalidOperationException("Pipe is not connected");
			}
			if (stream == null)
			{
				stream = new FileStream(handle.DangerousGetHandle(), (!CanRead) ? FileAccess.Write : ((!CanWrite) ? FileAccess.Read : FileAccess.ReadWrite), ownsHandle: false, buffer_size, IsAsync);
			}
			return stream;
		}
		set
		{
			stream = value;
		}
	}

	protected bool IsHandleExposed { get; private set; }

	[System.MonoTODO]
	public bool IsMessageComplete { get; private set; }

	[System.MonoTODO]
	public virtual int OutBufferSize => buffer_size;

	public virtual PipeTransmissionMode ReadMode
	{
		get
		{
			CheckPipePropertyOperations();
			return read_trans_mode;
		}
		set
		{
			CheckPipePropertyOperations();
			read_trans_mode = value;
		}
	}

	public SafePipeHandle SafePipeHandle
	{
		get
		{
			CheckPipePropertyOperations();
			return handle;
		}
	}

	public virtual PipeTransmissionMode TransmissionMode
	{
		get
		{
			CheckPipePropertyOperations();
			return transmission_mode;
		}
	}

	public override long Length
	{
		get
		{
			throw new NotSupportedException();
		}
	}

	public override long Position
	{
		get
		{
			return 0L;
		}
		set
		{
			throw new NotSupportedException();
		}
	}

	protected PipeStream(PipeDirection direction, int bufferSize)
		: this(direction, PipeTransmissionMode.Byte, bufferSize)
	{
	}

	protected PipeStream(PipeDirection direction, PipeTransmissionMode transmissionMode, int outBufferSize)
	{
		this.direction = direction;
		transmission_mode = transmissionMode;
		read_trans_mode = transmissionMode;
		if (outBufferSize <= 0)
		{
			throw new ArgumentOutOfRangeException("bufferSize must be greater than 0");
		}
		buffer_size = outBufferSize;
	}

	internal Exception ThrowACLException()
	{
		return new NotImplementedException("ACL is not supported in Mono");
	}

	internal static PipeAccessRights ToAccessRights(PipeDirection direction)
	{
		return direction switch
		{
			PipeDirection.In => PipeAccessRights.ReadData, 
			PipeDirection.Out => PipeAccessRights.WriteData, 
			PipeDirection.InOut => PipeAccessRights.ReadData | PipeAccessRights.WriteData, 
			_ => throw new ArgumentOutOfRangeException(), 
		};
	}

	internal static PipeDirection ToDirection(PipeAccessRights rights)
	{
		bool flag = (rights & PipeAccessRights.ReadData) != 0;
		bool flag2 = (rights & PipeAccessRights.WriteData) != 0;
		if (flag)
		{
			if (flag2)
			{
				return PipeDirection.InOut;
			}
			return PipeDirection.In;
		}
		if (flag2)
		{
			return PipeDirection.Out;
		}
		throw new ArgumentOutOfRangeException();
	}

	[System.MonoTODO]
	protected internal virtual void CheckPipePropertyOperations()
	{
	}

	[System.MonoTODO]
	protected internal void CheckReadOperations()
	{
		if (!IsConnected)
		{
			throw new InvalidOperationException("Pipe is not connected");
		}
		if (!CanRead)
		{
			throw new NotSupportedException("The pipe stream does not support read operations");
		}
	}

	[System.MonoTODO]
	protected internal void CheckWriteOperations()
	{
		if (!IsConnected)
		{
			throw new InvalidOperationException("Pipe us not connected");
		}
		if (!CanWrite)
		{
			throw new NotSupportedException("The pipe stream does not support write operations");
		}
	}

	protected void InitializeHandle(SafePipeHandle handle, bool isExposed, bool isAsync)
	{
		this.handle = handle;
		IsHandleExposed = isExposed;
		IsAsync = isAsync;
	}

	protected override void Dispose(bool disposing)
	{
		if (handle != null && disposing)
		{
			handle.Dispose();
		}
	}

	public override void SetLength(long value)
	{
		throw new NotSupportedException();
	}

	public override long Seek(long offset, SeekOrigin origin)
	{
		throw new NotSupportedException();
	}

	[System.MonoNotSupported("ACL is not supported in Mono")]
	public PipeSecurity GetAccessControl()
	{
		throw ThrowACLException();
	}

	[System.MonoNotSupported("ACL is not supported in Mono")]
	public void SetAccessControl(PipeSecurity pipeSecurity)
	{
		throw ThrowACLException();
	}

	public void WaitForPipeDrain()
	{
	}

	[System.MonoTODO]
	public override int Read(byte[] buffer, int offset, int count)
	{
		CheckReadOperations();
		return Stream.Read(buffer, offset, count);
	}

	[System.MonoTODO]
	public override int ReadByte()
	{
		CheckReadOperations();
		return Stream.ReadByte();
	}

	[System.MonoTODO]
	public override void Write(byte[] buffer, int offset, int count)
	{
		CheckWriteOperations();
		Stream.Write(buffer, offset, count);
	}

	[System.MonoTODO]
	public override void WriteByte(byte value)
	{
		CheckWriteOperations();
		Stream.WriteByte(value);
	}

	[System.MonoTODO]
	public override void Flush()
	{
		CheckWriteOperations();
		Stream.Flush();
	}

	[PermissionSet(SecurityAction.LinkDemand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\">\n<IPermission class=\"System.Security.Permissions.HostProtectionPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\nversion=\"1\"\nResources=\"None\"/>\n</PermissionSet>\n")]
	public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
	{
		if (read_delegate == null)
		{
			read_delegate = Read;
		}
		return read_delegate.BeginInvoke(buffer, offset, count, callback, state);
	}

	[PermissionSet(SecurityAction.LinkDemand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\">\n<IPermission class=\"System.Security.Permissions.HostProtectionPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\nversion=\"1\"\nResources=\"None\"/>\n</PermissionSet>\n")]
	public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
	{
		if (write_delegate == null)
		{
			write_delegate = Write;
		}
		return write_delegate.BeginInvoke(buffer, offset, count, callback, state);
	}

	public override int EndRead(IAsyncResult asyncResult)
	{
		return read_delegate.EndInvoke(asyncResult);
	}

	public override void EndWrite(IAsyncResult asyncResult)
	{
		write_delegate.EndInvoke(asyncResult);
	}
}
