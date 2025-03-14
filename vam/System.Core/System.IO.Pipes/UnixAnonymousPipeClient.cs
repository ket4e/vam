using Microsoft.Win32.SafeHandles;

namespace System.IO.Pipes;

internal class UnixAnonymousPipeClient : UnixAnonymousPipe, IPipe, IAnonymousPipeClient
{
	private SafePipeHandle handle;

	public override SafePipeHandle Handle => handle;

	public UnixAnonymousPipeClient(AnonymousPipeClientStream owner, SafePipeHandle handle)
	{
		this.handle = handle;
	}
}
