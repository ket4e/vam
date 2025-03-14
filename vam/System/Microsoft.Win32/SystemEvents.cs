using System;
using System.Collections;
using System.ComponentModel;
using System.Security.Permissions;
using System.Timers;

namespace Microsoft.Win32;

[PermissionSet((SecurityAction)14, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\"\nUnrestricted=\"true\"/>\n")]
public sealed class SystemEvents
{
	private static Hashtable TimerStore = new Hashtable();

	[System.MonoTODO]
	public static event EventHandler DisplaySettingsChanged
	{
		add
		{
		}
		remove
		{
		}
	}

	[System.MonoTODO("Currently does nothing on Mono")]
	public static event EventHandler DisplaySettingsChanging
	{
		add
		{
		}
		remove
		{
		}
	}

	[System.MonoTODO("Currently does nothing on Mono")]
	public static event EventHandler EventsThreadShutdown
	{
		add
		{
		}
		remove
		{
		}
	}

	[System.MonoTODO("Currently does nothing on Mono")]
	public static event EventHandler InstalledFontsChanged
	{
		add
		{
		}
		remove
		{
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	[Obsolete("")]
	[System.MonoTODO("Currently does nothing on Mono")]
	public static event EventHandler LowMemory
	{
		add
		{
		}
		remove
		{
		}
	}

	[System.MonoTODO("Currently does nothing on Mono")]
	public static event EventHandler PaletteChanged
	{
		add
		{
		}
		remove
		{
		}
	}

	[System.MonoTODO("Currently does nothing on Mono")]
	public static event PowerModeChangedEventHandler PowerModeChanged
	{
		add
		{
		}
		remove
		{
		}
	}

	[System.MonoTODO("Currently does nothing on Mono")]
	public static event SessionEndedEventHandler SessionEnded
	{
		add
		{
		}
		remove
		{
		}
	}

	[System.MonoTODO("Currently does nothing on Mono")]
	public static event SessionEndingEventHandler SessionEnding
	{
		add
		{
		}
		remove
		{
		}
	}

	[System.MonoTODO("Currently does nothing on Mono")]
	public static event SessionSwitchEventHandler SessionSwitch
	{
		add
		{
		}
		remove
		{
		}
	}

	[System.MonoTODO("Currently does nothing on Mono")]
	public static event EventHandler TimeChanged
	{
		add
		{
		}
		remove
		{
		}
	}

	public static event TimerElapsedEventHandler TimerElapsed;

	[System.MonoTODO("Currently does nothing on Mono")]
	public static event UserPreferenceChangedEventHandler UserPreferenceChanged
	{
		add
		{
		}
		remove
		{
		}
	}

	[System.MonoTODO("Currently does nothing on Mono")]
	public static event UserPreferenceChangingEventHandler UserPreferenceChanging
	{
		add
		{
		}
		remove
		{
		}
	}

	private SystemEvents()
	{
	}

	public static IntPtr CreateTimer(int interval)
	{
		int hashCode = Guid.NewGuid().GetHashCode();
		Timer timer = new Timer(interval);
		timer.Elapsed += InternalTimerElapsed;
		TimerStore.Add(hashCode, timer);
		return new IntPtr(hashCode);
	}

	public static void KillTimer(IntPtr timerId)
	{
		Timer timer = (Timer)TimerStore[timerId.GetHashCode()];
		timer.Stop();
		timer.Elapsed -= InternalTimerElapsed;
		timer.Dispose();
		TimerStore.Remove(timerId.GetHashCode());
	}

	private static void InternalTimerElapsed(object e, ElapsedEventArgs args)
	{
		if (SystemEvents.TimerElapsed != null)
		{
			SystemEvents.TimerElapsed(null, new TimerElapsedEventArgs(IntPtr.Zero));
		}
	}

	[System.MonoTODO]
	public static void InvokeOnEventsThread(Delegate method)
	{
		throw new NotImplementedException();
	}
}
