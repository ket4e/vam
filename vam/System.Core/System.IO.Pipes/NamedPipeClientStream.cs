using System.Security.Permissions;
using System.Security.Principal;
using Microsoft.Win32.SafeHandles;

namespace System.IO.Pipes;

[System.MonoTODO("working only on win32 right now")]
[PermissionSet(SecurityAction.LinkDemand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\">\n<IPermission class=\"System.Security.Permissions.HostProtectionPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\nversion=\"1\"\nResources=\"None\"/>\n</PermissionSet>\n")]
public sealed class NamedPipeClientStream : PipeStream
{
	private INamedPipeClient impl;

	public int NumberOfServerInstances
	{
		get
		{
			CheckPipePropertyOperations();
			return impl.NumberOfServerInstances;
		}
	}

	public NamedPipeClientStream(string pipeName)
		: this(".", pipeName)
	{
	}

	public NamedPipeClientStream(string serverName, string pipeName)
		: this(serverName, pipeName, PipeDirection.InOut)
	{
	}

	public NamedPipeClientStream(string serverName, string pipeName, PipeDirection direction)
		: this(serverName, pipeName, direction, PipeOptions.None)
	{
	}

	public NamedPipeClientStream(string serverName, string pipeName, PipeDirection direction, PipeOptions options)
		: this(serverName, pipeName, direction, options, TokenImpersonationLevel.None)
	{
	}

	public NamedPipeClientStream(string serverName, string pipeName, PipeDirection direction, PipeOptions options, TokenImpersonationLevel impersonationLevel)
		: this(serverName, pipeName, direction, options, impersonationLevel, HandleInheritability.None)
	{
	}

	public NamedPipeClientStream(string serverName, string pipeName, PipeDirection direction, PipeOptions options, TokenImpersonationLevel impersonationLevel, HandleInheritability inheritability)
		: this(serverName, pipeName, PipeStream.ToAccessRights(direction), options, impersonationLevel, inheritability)
	{
	}

	public NamedPipeClientStream(PipeDirection direction, bool isAsync, bool isConnected, SafePipeHandle safePipeHandle)
		: base(direction, 1024)
	{
		if (PipeStream.IsWindows)
		{
			impl = new Win32NamedPipeClient(this, safePipeHandle);
		}
		else
		{
			impl = new UnixNamedPipeClient(this, safePipeHandle);
		}
		base.IsConnected = isConnected;
		InitializeHandle(safePipeHandle, isExposed: true, isAsync);
	}

	public NamedPipeClientStream(string serverName, string pipeName, PipeAccessRights desiredAccessRights, PipeOptions options, TokenImpersonationLevel impersonationLevel, HandleInheritability inheritability)
		: base(PipeStream.ToDirection(desiredAccessRights), 1024)
	{
		if (impersonationLevel != 0 || inheritability != 0)
		{
			throw ThrowACLException();
		}
		if (PipeStream.IsWindows)
		{
			impl = new Win32NamedPipeClient(this, serverName, pipeName, desiredAccessRights, options, inheritability);
		}
		else
		{
			impl = new UnixNamedPipeClient(this, serverName, pipeName, desiredAccessRights, options, inheritability);
		}
	}

	public void Connect()
	{
		impl.Connect();
		InitializeHandle(impl.Handle, isExposed: false, impl.IsAsync);
		base.IsConnected = true;
	}

	public void Connect(int timeout)
	{
		impl.Connect(timeout);
		InitializeHandle(impl.Handle, isExposed: false, impl.IsAsync);
		base.IsConnected = true;
	}
}
