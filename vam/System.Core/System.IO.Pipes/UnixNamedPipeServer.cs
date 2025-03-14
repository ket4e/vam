using Microsoft.Win32.SafeHandles;
using Mono.Unix.Native;

namespace System.IO.Pipes;

internal class UnixNamedPipeServer : UnixNamedPipe, IPipe, INamedPipeServer
{
	private SafePipeHandle handle;

	private bool should_close_handle;

	public override SafePipeHandle Handle => handle;

	public UnixNamedPipeServer(NamedPipeServerStream owner, SafePipeHandle safePipeHandle)
	{
		handle = safePipeHandle;
	}

	public UnixNamedPipeServer(NamedPipeServerStream owner, string pipeName, int maxNumberOfServerInstances, PipeTransmissionMode transmissionMode, PipeAccessRights rights, PipeOptions options, int inBufferSize, int outBufferSize, HandleInheritability inheritability)
	{
		string text = Path.Combine("/var/tmp/", pipeName);
		EnsureTargetFile(text);
		string text2 = RightsToAccess(rights);
		ValidateOptions(options, owner.TransmissionMode);
		FileStream fileStream = new FileStream(text, FileMode.Open, RightsToFileAccess(rights), FileShare.ReadWrite);
		handle = new SafePipeHandle(fileStream.Handle, ownsHandle: false);
		owner.Stream = fileStream;
		should_close_handle = true;
	}

	public void Disconnect()
	{
		if (should_close_handle)
		{
			Stdlib.fclose(handle.DangerousGetHandle());
		}
	}

	public void WaitForConnection()
	{
	}
}
