using System.ComponentModel;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace System.IO.Pipes;

internal class Win32NamedPipeServer : Win32NamedPipe, IPipe, INamedPipeServer
{
	private SafePipeHandle handle;

	public override SafePipeHandle Handle => handle;

	public Win32NamedPipeServer(NamedPipeServerStream owner, SafePipeHandle safePipeHandle)
	{
		handle = safePipeHandle;
	}

	public Win32NamedPipeServer(NamedPipeServerStream owner, string pipeName, int maxNumberOfServerInstances, PipeTransmissionMode transmissionMode, PipeAccessRights rights, PipeOptions options, int inBufferSize, int outBufferSize, HandleInheritability inheritability)
	{
		string name = $"\\\\.\\pipe\\{pipeName}";
		uint num = 0u;
		if ((rights & PipeAccessRights.ReadData) != 0)
		{
			num |= 1u;
		}
		if ((rights & PipeAccessRights.WriteData) != 0)
		{
			num |= 2u;
		}
		if ((options & PipeOptions.WriteThrough) != 0)
		{
			num |= 0x80000000u;
		}
		int num2 = 0;
		if ((owner.TransmissionMode & PipeTransmissionMode.Message) != 0)
		{
			num2 |= 4;
		}
		if ((options & PipeOptions.Asynchronous) != 0)
		{
			num2 |= 1;
		}
		SecurityAttributesHack securityAttributes = new SecurityAttributesHack(inheritability == HandleInheritability.Inheritable);
		IntPtr intPtr = Win32Marshal.CreateNamedPipe(name, num, num2, maxNumberOfServerInstances, outBufferSize, inBufferSize, 0, ref securityAttributes, IntPtr.Zero);
		if (intPtr == new IntPtr(-1L))
		{
			throw new Win32Exception(Marshal.GetLastWin32Error());
		}
		handle = new SafePipeHandle(intPtr, ownsHandle: true);
	}

	public void Disconnect()
	{
		Win32Marshal.DisconnectNamedPipe(Handle);
	}

	public void WaitForConnection()
	{
		if (!Win32Marshal.ConnectNamedPipe(Handle, IntPtr.Zero))
		{
			throw new Win32Exception(Marshal.GetLastWin32Error());
		}
	}
}
