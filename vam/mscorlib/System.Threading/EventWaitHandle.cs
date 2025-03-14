using System.IO;
using System.Runtime.InteropServices;
using System.Security.AccessControl;

namespace System.Threading;

[ComVisible(true)]
public class EventWaitHandle : WaitHandle
{
	private EventWaitHandle(IntPtr handle)
	{
		Handle = handle;
	}

	public EventWaitHandle(bool initialState, EventResetMode mode)
	{
		bool manual = IsManualReset(mode);
		Handle = NativeEventCalls.CreateEvent_internal(manual, initialState, null, out var _);
	}

	public EventWaitHandle(bool initialState, EventResetMode mode, string name)
	{
		bool manual = IsManualReset(mode);
		Handle = NativeEventCalls.CreateEvent_internal(manual, initialState, name, out var _);
	}

	public EventWaitHandle(bool initialState, EventResetMode mode, string name, out bool createdNew)
	{
		bool manual = IsManualReset(mode);
		Handle = NativeEventCalls.CreateEvent_internal(manual, initialState, name, out createdNew);
	}

	[MonoTODO("Implement access control")]
	public EventWaitHandle(bool initialState, EventResetMode mode, string name, out bool createdNew, EventWaitHandleSecurity eventSecurity)
	{
		bool manual = IsManualReset(mode);
		Handle = NativeEventCalls.CreateEvent_internal(manual, initialState, name, out createdNew);
	}

	private bool IsManualReset(EventResetMode mode)
	{
		if (mode < EventResetMode.AutoReset || mode > EventResetMode.ManualReset)
		{
			throw new ArgumentException("mode");
		}
		return mode == EventResetMode.ManualReset;
	}

	[MonoTODO]
	public EventWaitHandleSecurity GetAccessControl()
	{
		throw new NotImplementedException();
	}

	public static EventWaitHandle OpenExisting(string name)
	{
		return OpenExisting(name, EventWaitHandleRights.Modify | EventWaitHandleRights.Synchronize);
	}

	public unsafe static EventWaitHandle OpenExisting(string name, EventWaitHandleRights rights)
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
		IntPtr intPtr = NativeEventCalls.OpenEvent_internal(name, rights, out error);
		if (intPtr == (IntPtr)(void*)null)
		{
			switch (error)
			{
			case MonoIOError.ERROR_FILE_NOT_FOUND:
				throw new WaitHandleCannotBeOpenedException(Locale.GetText("Named Event handle does not exist: ") + name);
			case MonoIOError.ERROR_ACCESS_DENIED:
				throw new UnauthorizedAccessException();
			default:
				throw new IOException(Locale.GetText("Win32 IO error: ") + error);
			}
		}
		return new EventWaitHandle(intPtr);
	}

	public bool Reset()
	{
		CheckDisposed();
		return NativeEventCalls.ResetEvent_internal(Handle);
	}

	public bool Set()
	{
		CheckDisposed();
		return NativeEventCalls.SetEvent_internal(Handle);
	}

	[MonoTODO]
	public void SetAccessControl(EventWaitHandleSecurity eventSecurity)
	{
		throw new NotImplementedException();
	}
}
