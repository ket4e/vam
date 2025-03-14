namespace System.Windows.Forms.CarbonInternal;

internal class ApplicationHandler : EventHandlerBase, IEventHandler
{
	internal const uint kEventAppActivated = 1u;

	internal const uint kEventAppDeactivated = 2u;

	internal const uint kEventAppQuit = 3u;

	internal const uint kEventAppLaunchNotification = 4u;

	internal const uint kEventAppLaunched = 5u;

	internal const uint kEventAppTerminated = 6u;

	internal const uint kEventAppFrontSwitched = 7u;

	internal const uint kEventAppFocusMenuBar = 8u;

	internal const uint kEventAppFocusNextDocumentWindow = 9u;

	internal const uint kEventAppFocusNextFloatingWindow = 10u;

	internal const uint kEventAppFocusToolbar = 11u;

	internal const uint kEventAppFocusDrawer = 12u;

	internal const uint kEventAppGetDockTileMenu = 20u;

	internal const uint kEventAppIsEventInInstantMouser = 104u;

	internal const uint kEventAppHidden = 107u;

	internal const uint kEventAppShown = 108u;

	internal const uint kEventAppSystemUIModeChanged = 109u;

	internal const uint kEventAppAvailableWindowBoundsChanged = 110u;

	internal const uint kEventAppActiveWindowChanged = 111u;

	internal ApplicationHandler(XplatUICarbon driver)
		: base(driver)
	{
	}

	public bool ProcessEvent(IntPtr callref, IntPtr eventref, IntPtr handle, uint kind, ref MSG msg)
	{
		switch (kind)
		{
		case 1u:
			foreach (IntPtr utilityWindow in XplatUICarbon.UtilityWindows)
			{
				if (!XplatUICarbon.IsWindowVisible(utilityWindow))
				{
					XplatUICarbon.ShowWindow(utilityWindow);
				}
			}
			break;
		case 2u:
			if (XplatUICarbon.FocusWindow != IntPtr.Zero)
			{
				Driver.SendMessage(XplatUICarbon.FocusWindow, Msg.WM_KILLFOCUS, IntPtr.Zero, IntPtr.Zero);
			}
			if (XplatUICarbon.Grab.Hwnd != IntPtr.Zero)
			{
				Driver.SendMessage(Hwnd.ObjectFromHandle(XplatUICarbon.Grab.Hwnd).Handle, Msg.WM_LBUTTONDOWN, (IntPtr)1, (IntPtr)((Driver.MousePosition.X << 16) | Driver.MousePosition.Y));
			}
			foreach (IntPtr utilityWindow2 in XplatUICarbon.UtilityWindows)
			{
				if (XplatUICarbon.IsWindowVisible(utilityWindow2))
				{
					XplatUICarbon.HideWindow(utilityWindow2);
				}
			}
			break;
		}
		return true;
	}
}
