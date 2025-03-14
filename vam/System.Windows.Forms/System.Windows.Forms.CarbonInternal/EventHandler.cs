using System.Runtime.InteropServices;

namespace System.Windows.Forms.CarbonInternal;

internal class EventHandler
{
	internal const int EVENT_NOT_HANDLED = 0;

	internal const int EVENT_HANDLED = -9874;

	internal const uint kEventClassMouse = 1836021107u;

	internal const uint kEventClassKeyboard = 1801812322u;

	internal const uint kEventClassTextInput = 1952807028u;

	internal const uint kEventClassApplication = 1634758764u;

	internal const uint kEventClassAppleEvent = 1701867619u;

	internal const uint kEventClassMenu = 1835363957u;

	internal const uint kEventClassWindow = 2003398244u;

	internal const uint kEventClassControl = 1668183148u;

	internal const uint kEventClassCommand = 1668113523u;

	internal const uint kEventClassTablet = 1952607348u;

	internal const uint kEventClassVolume = 1987013664u;

	internal const uint kEventClassAppearance = 1634758765u;

	internal const uint kEventClassService = 1936028278u;

	internal const uint kEventClassToolbar = 1952604530u;

	internal const uint kEventClassToolbarItem = 1952606580u;

	internal const uint kEventClassAccessibility = 1633903461u;

	internal const uint kEventClassHIObject = 1751740258u;

	internal static EventDelegate EventHandlerDelegate = EventCallback;

	internal static XplatUICarbon Driver;

	internal static EventTypeSpec[] HIObjectEvents = new EventTypeSpec[3]
	{
		new EventTypeSpec(1751740258u, 1u),
		new EventTypeSpec(1751740258u, 2u),
		new EventTypeSpec(1751740258u, 3u)
	};

	internal static EventTypeSpec[] ControlEvents = new EventTypeSpec[9]
	{
		new EventTypeSpec(1668183148u, 154u),
		new EventTypeSpec(1668183148u, 4u),
		new EventTypeSpec(1668183148u, 18u),
		new EventTypeSpec(1668183148u, 19u),
		new EventTypeSpec(1668183148u, 20u),
		new EventTypeSpec(1668183148u, 21u),
		new EventTypeSpec(1668183148u, 8u),
		new EventTypeSpec(1668183148u, 1000u),
		new EventTypeSpec(1668183148u, 157u)
	};

	internal static EventTypeSpec[] ApplicationEvents = new EventTypeSpec[2]
	{
		new EventTypeSpec(1634758764u, 1u),
		new EventTypeSpec(1634758764u, 2u)
	};

	private static EventTypeSpec[] WindowEvents = new EventTypeSpec[23]
	{
		new EventTypeSpec(1836021107u, 5u),
		new EventTypeSpec(1836021107u, 6u),
		new EventTypeSpec(1836021107u, 1u),
		new EventTypeSpec(1836021107u, 2u),
		new EventTypeSpec(1836021107u, 10u),
		new EventTypeSpec(1836021107u, 11u),
		new EventTypeSpec(2003398244u, 6u),
		new EventTypeSpec(2003398244u, 5u),
		new EventTypeSpec(2003398244u, 6u),
		new EventTypeSpec(2003398244u, 67u),
		new EventTypeSpec(2003398244u, 86u),
		new EventTypeSpec(2003398244u, 70u),
		new EventTypeSpec(2003398244u, 87u),
		new EventTypeSpec(2003398244u, 27u),
		new EventTypeSpec(2003398244u, 28u),
		new EventTypeSpec(2003398244u, 29u),
		new EventTypeSpec(2003398244u, 72u),
		new EventTypeSpec(2003398244u, 24u),
		new EventTypeSpec(1801812322u, 4u),
		new EventTypeSpec(1801812322u, 1u),
		new EventTypeSpec(1801812322u, 2u),
		new EventTypeSpec(1801812322u, 3u),
		new EventTypeSpec(1952807028u, 2u)
	};

	internal static int EventCallback(IntPtr callref, IntPtr eventref, IntPtr handle)
	{
		uint eventClass = GetEventClass(eventref);
		uint eventKind = GetEventKind(eventref);
		MSG msg = default(MSG);
		IEventHandler eventHandler = null;
		switch (eventClass)
		{
		case 1751740258u:
			eventHandler = Driver.HIObjectHandler;
			break;
		case 1801812322u:
		case 1952807028u:
			eventHandler = Driver.KeyboardHandler;
			break;
		case 2003398244u:
			eventHandler = Driver.WindowHandler;
			break;
		case 1836021107u:
			eventHandler = Driver.MouseHandler;
			break;
		case 1668183148u:
			eventHandler = Driver.ControlHandler;
			break;
		case 1634758764u:
			eventHandler = Driver.ApplicationHandler;
			break;
		default:
			return 0;
		}
		if (eventHandler.ProcessEvent(callref, eventref, handle, eventKind, ref msg))
		{
			Driver.EnqueueMessage(msg);
			return -9874;
		}
		return 0;
	}

	internal static bool TranslateMessage(ref MSG msg)
	{
		bool flag = false;
		if (!flag)
		{
			flag = Driver.KeyboardHandler.TranslateMessage(ref msg);
		}
		if (!flag)
		{
			flag = Driver.MouseHandler.TranslateMessage(ref msg);
		}
		return flag;
	}

	internal static void InstallApplicationHandler()
	{
		InstallEventHandler(GetApplicationEventTarget(), EventHandlerDelegate, (uint)ApplicationEvents.Length, ApplicationEvents, IntPtr.Zero, IntPtr.Zero);
	}

	internal static void InstallControlHandler(IntPtr control)
	{
		InstallEventHandler(GetControlEventTarget(control), EventHandlerDelegate, (uint)ControlEvents.Length, ControlEvents, control, IntPtr.Zero);
	}

	internal static void InstallWindowHandler(IntPtr window)
	{
		InstallEventHandler(GetWindowEventTarget(window), EventHandlerDelegate, (uint)WindowEvents.Length, WindowEvents, window, IntPtr.Zero);
	}

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	private static extern IntPtr GetApplicationEventTarget();

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	internal static extern IntPtr GetControlEventTarget(IntPtr control);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	internal static extern IntPtr GetWindowEventTarget(IntPtr window);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	internal static extern uint GetEventClass(IntPtr eventref);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	private static extern uint GetEventKind(IntPtr eventref);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	private static extern int InstallEventHandler(IntPtr window, EventDelegate event_handler, uint count, EventTypeSpec[] types, IntPtr user_data, IntPtr handlerref);
}
