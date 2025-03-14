using System.Security.Permissions;
using Microsoft.Win32.SafeHandles;

namespace System.IO.Pipes;

[System.MonoTODO("working only on win32 right now")]
[PermissionSet(SecurityAction.LinkDemand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\">\n<IPermission class=\"System.Security.Permissions.HostProtectionPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\nversion=\"1\"\nResources=\"None\"/>\n</PermissionSet>\n")]
public sealed class NamedPipeServerStream : PipeStream
{
	[System.MonoTODO]
	public const int MaxAllowedServerInstances = 1;

	private INamedPipeServer impl;

	private Action wait_connect_delegate;

	public NamedPipeServerStream(string pipeName)
		: this(pipeName, PipeDirection.InOut)
	{
	}

	public NamedPipeServerStream(string pipeName, PipeDirection direction)
		: this(pipeName, direction, 1)
	{
	}

	public NamedPipeServerStream(string pipeName, PipeDirection direction, int maxNumberOfServerInstances)
		: this(pipeName, direction, maxNumberOfServerInstances, PipeTransmissionMode.Byte)
	{
	}

	public NamedPipeServerStream(string pipeName, PipeDirection direction, int maxNumberOfServerInstances, PipeTransmissionMode transmissionMode)
		: this(pipeName, direction, maxNumberOfServerInstances, transmissionMode, PipeOptions.None)
	{
	}

	public NamedPipeServerStream(string pipeName, PipeDirection direction, int maxNumberOfServerInstances, PipeTransmissionMode transmissionMode, PipeOptions options)
		: this(pipeName, direction, maxNumberOfServerInstances, transmissionMode, options, 1024, 1024)
	{
	}

	public NamedPipeServerStream(string pipeName, PipeDirection direction, int maxNumberOfServerInstances, PipeTransmissionMode transmissionMode, PipeOptions options, int inBufferSize, int outBufferSize)
		: this(pipeName, direction, maxNumberOfServerInstances, transmissionMode, options, inBufferSize, outBufferSize, null)
	{
	}

	public NamedPipeServerStream(string pipeName, PipeDirection direction, int maxNumberOfServerInstances, PipeTransmissionMode transmissionMode, PipeOptions options, int inBufferSize, int outBufferSize, PipeSecurity pipeSecurity)
		: this(pipeName, direction, maxNumberOfServerInstances, transmissionMode, options, inBufferSize, outBufferSize, pipeSecurity, HandleInheritability.None)
	{
	}

	public NamedPipeServerStream(string pipeName, PipeDirection direction, int maxNumberOfServerInstances, PipeTransmissionMode transmissionMode, PipeOptions options, int inBufferSize, int outBufferSize, PipeSecurity pipeSecurity, HandleInheritability inheritability)
		: this(pipeName, direction, maxNumberOfServerInstances, transmissionMode, options, inBufferSize, outBufferSize, pipeSecurity, inheritability, PipeAccessRights.ReadData | PipeAccessRights.WriteData)
	{
	}

	[System.MonoTODO]
	public NamedPipeServerStream(string pipeName, PipeDirection direction, int maxNumberOfServerInstances, PipeTransmissionMode transmissionMode, PipeOptions options, int inBufferSize, int outBufferSize, PipeSecurity pipeSecurity, HandleInheritability inheritability, PipeAccessRights additionalAccessRights)
		: base(direction, transmissionMode, outBufferSize)
	{
		if (pipeSecurity != null)
		{
			throw ThrowACLException();
		}
		PipeAccessRights rights = PipeStream.ToAccessRights(direction) | additionalAccessRights;
		if (PipeStream.IsWindows)
		{
			impl = new Win32NamedPipeServer(this, pipeName, maxNumberOfServerInstances, transmissionMode, rights, options, inBufferSize, outBufferSize, inheritability);
		}
		else
		{
			impl = new UnixNamedPipeServer(this, pipeName, maxNumberOfServerInstances, transmissionMode, rights, options, inBufferSize, outBufferSize, inheritability);
		}
		InitializeHandle(impl.Handle, isExposed: false, (options & PipeOptions.Asynchronous) != 0);
	}

	public NamedPipeServerStream(PipeDirection direction, bool isAsync, bool isConnected, SafePipeHandle safePipeHandle)
		: base(direction, 1024)
	{
		if (PipeStream.IsWindows)
		{
			impl = new Win32NamedPipeServer(this, safePipeHandle);
		}
		else
		{
			impl = new UnixNamedPipeServer(this, safePipeHandle);
		}
		base.IsConnected = isConnected;
		InitializeHandle(safePipeHandle, isExposed: true, isAsync);
	}

	public void Disconnect()
	{
		impl.Disconnect();
	}

	[System.MonoTODO]
	[PermissionSet(SecurityAction.Demand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\">\n<IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\nversion=\"1\"\nFlags=\"ControlPrincipal\"/>\n</PermissionSet>\n")]
	public void RunAsClient(PipeStreamImpersonationWorker impersonationWorker)
	{
		throw new NotImplementedException();
	}

	public void WaitForConnection()
	{
		impl.WaitForConnection();
		base.IsConnected = true;
	}

	[System.MonoTODO]
	[PermissionSet(SecurityAction.Demand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\">\n<IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\nversion=\"1\"\nFlags=\"ControlPrincipal\"/>\n</PermissionSet>\n")]
	public string GetImpersonationUserName()
	{
		throw new NotImplementedException();
	}

	[PermissionSet(SecurityAction.LinkDemand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\">\n<IPermission class=\"System.Security.Permissions.HostProtectionPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\nversion=\"1\"\nResources=\"None\"/>\n</PermissionSet>\n")]
	public IAsyncResult BeginWaitForConnection(AsyncCallback callback, object state)
	{
		if (wait_connect_delegate == null)
		{
			wait_connect_delegate = WaitForConnection;
		}
		return wait_connect_delegate.BeginInvoke(callback, state);
	}

	public void EndWaitForConnection(IAsyncResult asyncResult)
	{
		wait_connect_delegate.EndInvoke(asyncResult);
	}
}
