using System.Globalization;
using System.Security.Permissions;
using Microsoft.Win32.SafeHandles;

namespace System.IO.Pipes;

[System.MonoTODO("Anonymous pipes are not working even on win32, due to some access authorization issue")]
[PermissionSet(SecurityAction.LinkDemand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\">\n<IPermission class=\"System.Security.Permissions.HostProtectionPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\nversion=\"1\"\nResources=\"None\"/>\n</PermissionSet>\n")]
public sealed class AnonymousPipeServerStream : PipeStream
{
	private IAnonymousPipeServer impl;

	[System.MonoTODO]
	public SafePipeHandle ClientSafePipeHandle { get; private set; }

	public override PipeTransmissionMode ReadMode
	{
		set
		{
			if (value == PipeTransmissionMode.Message)
			{
				throw new NotSupportedException();
			}
		}
	}

	public override PipeTransmissionMode TransmissionMode => PipeTransmissionMode.Byte;

	public AnonymousPipeServerStream()
		: this(PipeDirection.Out)
	{
	}

	public AnonymousPipeServerStream(PipeDirection direction)
		: this(direction, HandleInheritability.None)
	{
	}

	public AnonymousPipeServerStream(PipeDirection direction, HandleInheritability inheritability)
		: this(direction, inheritability, 1024)
	{
	}

	public AnonymousPipeServerStream(PipeDirection direction, HandleInheritability inheritability, int bufferSize)
		: this(direction, inheritability, bufferSize, null)
	{
	}

	public AnonymousPipeServerStream(PipeDirection direction, HandleInheritability inheritability, int bufferSize, PipeSecurity pipeSecurity)
		: base(direction, bufferSize)
	{
		if (pipeSecurity != null)
		{
			throw ThrowACLException();
		}
		if (direction == PipeDirection.InOut)
		{
			throw new NotSupportedException("Anonymous pipe direction can only be either in or out.");
		}
		if (PipeStream.IsWindows)
		{
			impl = new Win32AnonymousPipeServer(this, direction, inheritability, bufferSize);
		}
		else
		{
			impl = new UnixAnonymousPipeServer(this, direction, inheritability, bufferSize);
		}
		InitializeHandle(impl.Handle, isExposed: false, isAsync: false);
		base.IsConnected = true;
	}

	[System.MonoTODO]
	public AnonymousPipeServerStream(PipeDirection direction, SafePipeHandle serverSafePipeHandle, SafePipeHandle clientSafePipeHandle)
		: base(direction, 1024)
	{
		if (serverSafePipeHandle == null)
		{
			throw new ArgumentNullException("serverSafePipeHandle");
		}
		if (clientSafePipeHandle == null)
		{
			throw new ArgumentNullException("clientSafePipeHandle");
		}
		if (direction == PipeDirection.InOut)
		{
			throw new NotSupportedException("Anonymous pipe direction can only be either in or out.");
		}
		if (PipeStream.IsWindows)
		{
			impl = new Win32AnonymousPipeServer(this, serverSafePipeHandle, clientSafePipeHandle);
		}
		else
		{
			impl = new UnixAnonymousPipeServer(this, serverSafePipeHandle, clientSafePipeHandle);
		}
		InitializeHandle(serverSafePipeHandle, isExposed: true, isAsync: false);
		base.IsConnected = true;
		ClientSafePipeHandle = clientSafePipeHandle;
	}

	[System.MonoTODO]
	public void DisposeLocalCopyOfClientHandle()
	{
		impl.DisposeLocalCopyOfClientHandle();
	}

	public string GetClientHandleAsString()
	{
		return impl.Handle.DangerousGetHandle().ToInt64().ToString(NumberFormatInfo.InvariantInfo);
	}
}
