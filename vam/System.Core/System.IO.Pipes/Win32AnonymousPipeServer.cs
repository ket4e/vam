using System.ComponentModel;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace System.IO.Pipes;

internal class Win32AnonymousPipeServer : Win32AnonymousPipe, IPipe, IAnonymousPipeServer
{
	private SafePipeHandle server_handle;

	private SafePipeHandle client_handle;

	public override SafePipeHandle Handle => server_handle;

	public SafePipeHandle ClientHandle => client_handle;

	public Win32AnonymousPipeServer(AnonymousPipeServerStream owner, PipeDirection direction, HandleInheritability inheritability, int bufferSize)
	{
		SecurityAttributesHack pipeAtts = new SecurityAttributesHack(inheritability == HandleInheritability.Inheritable);
		if (!Win32Marshal.CreatePipe(out var readHandle, out var writeHandle, ref pipeAtts, bufferSize))
		{
			throw new Win32Exception(Marshal.GetLastWin32Error());
		}
		SafePipeHandle safePipeHandle = new SafePipeHandle(readHandle, ownsHandle: true);
		SafePipeHandle safePipeHandle2 = new SafePipeHandle(writeHandle, ownsHandle: true);
		if (direction == PipeDirection.Out)
		{
			server_handle = safePipeHandle2;
			client_handle = safePipeHandle;
		}
		else
		{
			server_handle = safePipeHandle;
			client_handle = safePipeHandle2;
		}
	}

	public Win32AnonymousPipeServer(AnonymousPipeServerStream owner, SafePipeHandle serverHandle, SafePipeHandle clientHandle)
	{
		server_handle = serverHandle;
		client_handle = clientHandle;
	}

	public void DisposeLocalCopyOfClientHandle()
	{
		throw new NotImplementedException();
	}
}
