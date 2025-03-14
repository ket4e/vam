using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Permissions;

namespace System.Threading;

[ComVisible(true)]
public sealed class Mutex : WaitHandle
{
	private Mutex(IntPtr handle)
	{
		Handle = handle;
	}

	[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
	public Mutex()
	{
		Handle = CreateMutex_internal(initiallyOwned: false, null, out var _);
	}

	[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
	public Mutex(bool initiallyOwned)
	{
		Handle = CreateMutex_internal(initiallyOwned, null, out var _);
	}

	[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
	[PermissionSet(SecurityAction.LinkDemand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Flags=\"UnmanagedCode\"/>\n</PermissionSet>\n")]
	public Mutex(bool initiallyOwned, string name)
	{
		Handle = CreateMutex_internal(initiallyOwned, name, out var _);
	}

	[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
	[PermissionSet(SecurityAction.LinkDemand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Flags=\"UnmanagedCode\"/>\n</PermissionSet>\n")]
	public Mutex(bool initiallyOwned, string name, out bool createdNew)
	{
		Handle = CreateMutex_internal(initiallyOwned, name, out createdNew);
	}

	[MonoTODO("Implement MutexSecurity")]
	[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
	public Mutex(bool initiallyOwned, string name, out bool createdNew, MutexSecurity mutexSecurity)
	{
		Handle = CreateMutex_internal(initiallyOwned, name, out createdNew);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr CreateMutex_internal(bool initiallyOwned, string name, out bool created);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool ReleaseMutex_internal(IntPtr handle);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr OpenMutex_internal(string name, MutexRights rights, out MonoIOError error);

	public MutexSecurity GetAccessControl()
	{
		throw new NotImplementedException();
	}

	public static Mutex OpenExisting(string name)
	{
		return OpenExisting(name, MutexRights.Modify | MutexRights.Synchronize);
	}

	public unsafe static Mutex OpenExisting(string name, MutexRights rights)
	{
		if (name == null)
		{
			throw new ArgumentNullException("name");
		}
		if (name.Length == 0 || name.Length > 260)
		{
			throw new ArgumentException("name", Locale.GetText("Invalid length [1-260]."));
		}
		MonoIOError error;
		IntPtr intPtr = OpenMutex_internal(name, rights, out error);
		if (intPtr == (IntPtr)(void*)null)
		{
			switch (error)
			{
			case MonoIOError.ERROR_FILE_NOT_FOUND:
				throw new WaitHandleCannotBeOpenedException(Locale.GetText("Named Mutex handle does not exist: ") + name);
			case MonoIOError.ERROR_ACCESS_DENIED:
				throw new UnauthorizedAccessException();
			default:
				throw new IOException(Locale.GetText("Win32 IO error: ") + error);
			}
		}
		return new Mutex(intPtr);
	}

	[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
	public void ReleaseMutex()
	{
		if (!ReleaseMutex_internal(Handle))
		{
			throw new ApplicationException("Mutex is not owned");
		}
	}

	public void SetAccessControl(MutexSecurity mutexSecurity)
	{
		throw new NotImplementedException();
	}
}
