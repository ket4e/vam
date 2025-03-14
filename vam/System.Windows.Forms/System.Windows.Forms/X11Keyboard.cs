using System.Collections;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;

namespace System.Windows.Forms;

internal class X11Keyboard : IDisposable
{
	private class XIMCallbackContext
	{
		private XIMCallback startCB;

		private XIMCallback doneCB;

		private XIMCallback drawCB;

		private XIMCallback caretCB;

		private IntPtr pStartCB = IntPtr.Zero;

		private IntPtr pDoneCB = IntPtr.Zero;

		private IntPtr pDrawCB = IntPtr.Zero;

		private IntPtr pCaretCB = IntPtr.Zero;

		private IntPtr pStartCBN = IntPtr.Zero;

		private IntPtr pDoneCBN = IntPtr.Zero;

		private IntPtr pDrawCBN = IntPtr.Zero;

		private IntPtr pCaretCBN = IntPtr.Zero;

		public XIMCallbackContext(IntPtr clientWindow)
		{
			startCB = new XIMCallback(IntPtr.Zero, DoPreeditStart);
			doneCB = new XIMCallback(IntPtr.Zero, DoPreeditDone);
			drawCB = new XIMCallback(IntPtr.Zero, DoPreeditDraw);
			caretCB = new XIMCallback(IntPtr.Zero, DoPreeditCaret);
			pStartCB = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(XIMCallback)));
			pDoneCB = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(XIMCallback)));
			pDrawCB = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(XIMCallback)));
			pCaretCB = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(XIMCallback)));
			pStartCBN = Marshal.StringToHGlobalAnsi("preeditStartCallback");
			pDoneCBN = Marshal.StringToHGlobalAnsi("preeditDoneCallback");
			pDrawCBN = Marshal.StringToHGlobalAnsi("preeditDrawCallback");
			pCaretCBN = Marshal.StringToHGlobalAnsi("preeditCaretCallback");
		}

		~XIMCallbackContext()
		{
			if (pStartCBN != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(pStartCBN);
			}
			if (pDoneCBN != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(pDoneCBN);
			}
			if (pDrawCBN != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(pDrawCBN);
			}
			if (pCaretCBN != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(pCaretCBN);
			}
			if (pStartCB != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(pStartCB);
			}
			if (pDoneCB != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(pDoneCB);
			}
			if (pDrawCB != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(pDrawCB);
			}
			if (pCaretCB != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(pCaretCB);
			}
		}

		private int DoPreeditStart(IntPtr xic, IntPtr clientData, IntPtr callData)
		{
			Console.WriteLine("DoPreeditStart");
			return 100;
		}

		private int DoPreeditDone(IntPtr xic, IntPtr clientData, IntPtr callData)
		{
			Console.WriteLine("DoPreeditDone");
			return 0;
		}

		private int DoPreeditDraw(IntPtr xic, IntPtr clientData, IntPtr callData)
		{
			Console.WriteLine("DoPreeditDraw");
			return 0;
		}

		private int DoPreeditCaret(IntPtr xic, IntPtr clientData, IntPtr callData)
		{
			Console.WriteLine("DoPreeditCaret");
			return 0;
		}

		public IntPtr CreateXic(IntPtr window, IntPtr xim)
		{
			Marshal.StructureToPtr(startCB, pStartCB, fDeleteOld: false);
			Marshal.StructureToPtr(doneCB, pDoneCB, fDeleteOld: false);
			Marshal.StructureToPtr(drawCB, pDrawCB, fDeleteOld: false);
			Marshal.StructureToPtr(caretCB, pCaretCB, fDeleteOld: false);
			IntPtr value = XVaCreateNestedList(0, pStartCBN, pStartCB, pDoneCBN, pDoneCB, pDrawCBN, pDrawCB, pCaretCBN, pCaretCB, IntPtr.Zero);
			return XCreateIC(xim, "inputStyle", XIMProperties.XIMPreeditCallbacks | XIMProperties.XIMStatusNothing, "clientWindow", window, "preeditAttributes", value, IntPtr.Zero);
		}
	}

	private class XIMPositionContext
	{
		public CaretStruct Caret;

		public int X;

		public int Y;
	}

	private const XIMProperties styleRoot = XIMProperties.XIMPreeditNothing | XIMProperties.XIMStatusNothing;

	private const XIMProperties styleOverTheSpot = XIMProperties.XIMPreeditPosition | XIMProperties.XIMStatusNothing;

	private const XIMProperties styleOnTheSpot = XIMProperties.XIMPreeditCallbacks | XIMProperties.XIMStatusNothing;

	private const string ENV_NAME_XIM_STYLE = "MONO_WINFORMS_XIM_STYLE";

	internal static object XlibLock;

	private IntPtr display;

	private IntPtr client_window;

	private IntPtr xim;

	private Hashtable xic_table = new Hashtable();

	private XIMPositionContext positionContext;

	private XIMCallbackContext callbackContext;

	private XIMProperties ximStyle;

	private EventMask xic_event_mask;

	private StringBuilder lookup_buffer;

	private byte[] utf8_buffer;

	private int min_keycode;

	private int max_keycode;

	private int keysyms_per_keycode;

	private int syms;

	private int[] keyc2vkey = new int[256];

	private int[] keyc2scan = new int[256];

	private byte[] key_state_table = new byte[256];

	private int lcid;

	private bool num_state;

	private bool cap_state;

	private bool initialized;

	private bool menu_state;

	private int NumLockMask;

	private int AltGrMask;

	private string stored_keyevent_string;

	private static readonly int[] nonchar_key_vkey = new int[256]
	{
		0, 0, 0, 0, 0, 0, 0, 0, 8, 9,
		0, 12, 0, 13, 0, 0, 0, 0, 0, 19,
		145, 0, 0, 0, 0, 0, 0, 27, 0, 0,
		0, 0, 0, 0, 29, 28, 0, 0, 0, 0,
		0, 0, 243, 0, 0, 0, 0, 0, 0, 0,
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		36, 37, 38, 39, 40, 33, 34, 35, 0, 0,
		0, 0, 0, 0, 0, 0, 41, 44, 43, 45,
		0, 0, 0, 0, 3, 47, 3, 3, 0, 0,
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		0, 0, 0, 0, 0, 0, 0, 144, 0, 0,
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		0, 13, 0, 0, 0, 0, 0, 0, 0, 36,
		37, 38, 39, 40, 33, 34, 35, 0, 45, 46,
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		106, 107, 108, 109, 110, 111, 96, 97, 98, 99,
		100, 101, 102, 103, 104, 105, 0, 0, 0, 0,
		112, 113, 114, 115, 116, 117, 118, 119, 120, 121,
		122, 123, 124, 125, 126, 127, 0, 0, 0, 0,
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		0, 0, 0, 0, 0, 16, 16, 17, 17, 20,
		0, 18, 18, 18, 18, 0, 0, 0, 0, 0,
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		0, 0, 0, 0, 0, 46
	};

	private static readonly int[] nonchar_key_scan = new int[256]
	{
		0, 0, 0, 0, 0, 0, 0, 0, 14, 15,
		0, 0, 0, 28, 0, 0, 0, 0, 0, 69,
		70, 0, 0, 0, 0, 0, 0, 1, 0, 0,
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		327, 331, 328, 333, 336, 329, 337, 335, 0, 0,
		0, 0, 0, 0, 0, 0, 0, 311, 0, 338,
		0, 0, 0, 0, 0, 0, 56, 326, 0, 0,
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		0, 0, 0, 0, 0, 0, 312, 325, 0, 0,
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		0, 284, 0, 0, 0, 0, 0, 0, 0, 71,
		75, 72, 77, 80, 73, 81, 79, 76, 82, 83,
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		55, 78, 0, 74, 83, 309, 82, 79, 80, 81,
		75, 76, 77, 71, 72, 73, 0, 0, 0, 0,
		59, 60, 61, 62, 63, 64, 65, 66, 67, 68,
		87, 88, 0, 0, 0, 0, 0, 0, 0, 0,
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		0, 0, 0, 0, 0, 42, 54, 29, 285, 58,
		0, 56, 312, 56, 312, 0, 0, 0, 0, 0,
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		0, 0, 0, 0, 0, 339
	};

	private static readonly int[] nonchar_vkey_key;

	public IntPtr ClientWindow => client_window;

	public EventMask KeyEventMask => xic_event_mask;

	public Keys ModifierKeys
	{
		get
		{
			Keys keys = Keys.None;
			if ((key_state_table[16] & 0x80u) != 0)
			{
				keys |= Keys.Shift;
			}
			if ((key_state_table[17] & 0x80u) != 0)
			{
				keys |= Keys.Control;
			}
			if ((key_state_table[18] & 0x80u) != 0)
			{
				keys |= Keys.Alt;
			}
			return keys;
		}
	}

	public X11Keyboard(IntPtr display, IntPtr clientWindow)
	{
		this.display = display;
		lookup_buffer = new StringBuilder(24);
		EnsureLayoutInitialized();
	}

	static X11Keyboard()
	{
		int[] array = new int[256];
		array[8] = 65288;
		array[9] = 65289;
		array[12] = 65291;
		array[13] = 65293;
		array[16] = 65505;
		array[17] = 65507;
		array[18] = 65383;
		array[20] = 65509;
		array[35] = 65367;
		array[36] = 65360;
		array[37] = 65361;
		array[38] = 65362;
		array[39] = 65363;
		array[40] = 65364;
		array[91] = 65511;
		array[92] = 65512;
		array[160] = 65505;
		array[161] = 65506;
		array[162] = 65507;
		array[163] = 65508;
		array[164] = 65513;
		array[165] = 65514;
		nonchar_vkey_key = array;
	}

	void IDisposable.Dispose()
	{
		if (!(xim != IntPtr.Zero))
		{
			return;
		}
		foreach (IntPtr value in xic_table.Values)
		{
			XDestroyIC(value);
		}
		xic_table.Clear();
		XCloseIM(xim);
		xim = IntPtr.Zero;
	}

	public void DestroyICForWindow(IntPtr window)
	{
		IntPtr xic = GetXic(window);
		if (xic != IntPtr.Zero)
		{
			xic_table.Remove((long)window);
			XDestroyIC(xic);
		}
	}

	public void EnsureLayoutInitialized()
	{
		if (!initialized)
		{
			KeyboardLayouts layouts = new KeyboardLayouts();
			KeyboardLayout keyboardLayout = DetectLayout(layouts);
			lcid = keyboardLayout.Lcid;
			CreateConversionArray(layouts, keyboardLayout);
			SetupXIM();
			initialized = true;
		}
	}

	private void SetupXIM()
	{
		xim = IntPtr.Zero;
		if (!XSupportsLocale())
		{
			Console.Error.WriteLine("X does not support your locale");
		}
		else if (!XSetLocaleModifiers(string.Empty))
		{
			Console.Error.WriteLine("Could not set X locale modifiers");
		}
		else if (!(Environment.GetEnvironmentVariable("MONO_WINFORMS_XIM_STYLE") == "disabled"))
		{
			xim = XOpenIM(display, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
			if (xim == IntPtr.Zero)
			{
				Console.Error.WriteLine("Could not get XIM");
			}
			else
			{
				utf8_buffer = new byte[100];
			}
			initialized = true;
		}
	}

	private void CreateXicForWindow(IntPtr window)
	{
		IntPtr intPtr = CreateXic(window, xim);
		xic_table[(long)window] = intPtr;
		if (intPtr == IntPtr.Zero)
		{
			Console.Error.WriteLine("Could not get XIC");
			return;
		}
		if (XGetICValues(intPtr, "filterEvents", out xic_event_mask, IntPtr.Zero) != null)
		{
			Console.Error.WriteLine("Could not get XIC values");
		}
		EventMask eventMask = EventMask.KeyPressMask | EventMask.ExposureMask | EventMask.FocusChangeMask;
		if ((xic_event_mask | eventMask) != xic_event_mask)
		{
			return;
		}
		xic_event_mask |= eventMask;
		lock (XlibLock)
		{
			XplatUIX11.XSelectInput(display, window, new IntPtr((int)xic_event_mask));
		}
	}

	private IntPtr GetXic(IntPtr window)
	{
		if (xim != IntPtr.Zero && xic_table.ContainsKey((long)window))
		{
			return (IntPtr)xic_table[(long)window];
		}
		return IntPtr.Zero;
	}

	private bool FilterKey(XEvent e, int vkey)
	{
		if (XplatUI.key_filters.Count == 0)
		{
			return false;
		}
		KeyFilterData key = default(KeyFilterData);
		key.Down = e.type == XEventName.KeyPress;
		key.ModifierKeys = ModifierKeys;
		LookupString(ref e, 0, out var keysym, out var _);
		key.keysym = (int)keysym;
		key.keycode = e.KeyEvent.keycode;
		key.str = lookup_buffer.ToString(0, lookup_buffer.Length);
		return XplatUI.FilterKey(key);
	}

	public void FocusIn(IntPtr window)
	{
		if (!(xim == IntPtr.Zero))
		{
			client_window = window;
			if (!xic_table.ContainsKey((long)window))
			{
				CreateXicForWindow(window);
			}
			IntPtr xic = GetXic(window);
			if (xic != IntPtr.Zero)
			{
				XSetICFocus(xic);
			}
		}
	}

	public void FocusOut(IntPtr window)
	{
		if (!(xim == IntPtr.Zero))
		{
			client_window = IntPtr.Zero;
			IntPtr xic = GetXic(window);
			if (xic != IntPtr.Zero)
			{
				Xutf8ResetIC(xic);
				XUnsetICFocus(xic);
			}
		}
	}

	public bool ResetKeyState(IntPtr hwnd, ref MSG msg)
	{
		if ((key_state_table[16] & 0x80u) != 0)
		{
			key_state_table[16] &= 127;
		}
		if ((key_state_table[17] & 0x80u) != 0)
		{
			key_state_table[17] &= 127;
		}
		if ((key_state_table[18] & 0x80u) != 0)
		{
			key_state_table[18] &= 127;
		}
		return false;
	}

	public void PreFilter(XEvent xevent)
	{
		if (xevent.KeyEvent.keycode >= keyc2vkey.Length)
		{
			return;
		}
		int num = keyc2vkey[xevent.KeyEvent.keycode];
		switch (xevent.type)
		{
		case XEventName.KeyRelease:
			key_state_table[num & 0xFF] &= 127;
			break;
		case XEventName.KeyPress:
			if ((key_state_table[num & 0xFF] & 0x80) == 0)
			{
				key_state_table[num & 0xFF] ^= 1;
			}
			key_state_table[num & 0xFF] |= 128;
			break;
		}
	}

	public void KeyEvent(IntPtr hwnd, XEvent xevent, ref MSG msg)
	{
		IntPtr status = IntPtr.Zero;
		XKeySym keysym;
		int num = LookupString(ref xevent, 24, out keysym, out status);
		if (((int)keysym >= 65025 && (int)keysym <= 65039) || keysym == (XKeySym)65406u)
		{
			UpdateKeyState(xevent);
			return;
		}
		if (xevent.KeyEvent.keycode >> 8 == 16)
		{
			xevent.KeyEvent.keycode = xevent.KeyEvent.keycode & 0xFF;
		}
		int num2 = (int)xevent.KeyEvent.time;
		if (status == (IntPtr)2)
		{
			msg = SendImeComposition(lookup_buffer.ToString(0, lookup_buffer.Length));
			msg.hwnd = hwnd;
			return;
		}
		AltGrMask = xevent.KeyEvent.state & 0x60F8;
		int num3 = EventToVkey(xevent);
		if (num3 == 0 && num != 0)
		{
			num3 = 252;
		}
		if (FilterKey(xevent, num3))
		{
			return;
		}
		switch ((VirtualKeys)(num3 & 0xFF))
		{
		case VirtualKeys.VK_NUMLOCK:
			GenerateMessage(VirtualKeys.VK_NUMLOCK, 69, xevent.KeyEvent.keycode, xevent.type, num2);
			return;
		case VirtualKeys.VK_CAPITAL:
			GenerateMessage(VirtualKeys.VK_CAPITAL, 58, xevent.KeyEvent.keycode, xevent.type, num2);
			return;
		}
		if ((key_state_table[144] & 1) == 0 != ((xevent.KeyEvent.state & NumLockMask) == 0))
		{
			GenerateMessage(VirtualKeys.VK_NUMLOCK, 69, xevent.KeyEvent.keycode, XEventName.KeyPress, num2);
			GenerateMessage(VirtualKeys.VK_NUMLOCK, 69, xevent.KeyEvent.keycode, XEventName.KeyRelease, num2);
		}
		if ((key_state_table[20] & 1) == 0 != ((xevent.KeyEvent.state & 2) == 0))
		{
			GenerateMessage(VirtualKeys.VK_CAPITAL, 58, xevent.KeyEvent.keycode, XEventName.KeyPress, num2);
			GenerateMessage(VirtualKeys.VK_CAPITAL, 58, xevent.KeyEvent.keycode, XEventName.KeyRelease, num2);
		}
		num_state = false;
		cap_state = false;
		int scan = keyc2scan[xevent.KeyEvent.keycode] & 0xFF;
		KeybdEventFlags keybdEventFlags = KeybdEventFlags.None;
		if (xevent.type == XEventName.KeyRelease)
		{
			keybdEventFlags |= KeybdEventFlags.KeyUp;
		}
		if (((uint)num3 & 0x100u) != 0)
		{
			keybdEventFlags |= KeybdEventFlags.ExtendedKey;
		}
		msg = SendKeyboardInput((VirtualKeys)(num3 & 0xFF), scan, xevent.KeyEvent.keycode, keybdEventFlags, num2);
		msg.hwnd = hwnd;
	}

	public bool TranslateMessage(ref MSG msg)
	{
		bool result = false;
		if (msg.message >= Msg.WM_KEYDOWN && msg.message <= Msg.WM_KEYLAST)
		{
			result = true;
		}
		if (msg.message == Msg.WM_SYSKEYUP && msg.wParam == (IntPtr)18 && menu_state)
		{
			msg.message = Msg.WM_KEYUP;
			menu_state = false;
		}
		if (msg.message != Msg.WM_KEYDOWN && msg.message != Msg.WM_SYSKEYDOWN)
		{
			return result;
		}
		if ((key_state_table[18] & 0x80u) != 0 && msg.wParam != (IntPtr)18)
		{
			menu_state = true;
		}
		EnsureLayoutInitialized();
		string buffer;
		switch (ToUnicode((int)msg.wParam, Control.HighOrder((int)msg.lParam), out buffer))
		{
		case 1:
		{
			Msg message = ((msg.message != Msg.WM_KEYDOWN) ? Msg.WM_SYSCHAR : Msg.WM_CHAR);
			XplatUI.PostMessage(msg.hwnd, message, (IntPtr)buffer[0], msg.lParam);
			break;
		}
		case -1:
		{
			Msg message = ((msg.message != Msg.WM_KEYDOWN) ? Msg.WM_SYSDEADCHAR : Msg.WM_DEADCHAR);
			XplatUI.PostMessage(msg.hwnd, message, (IntPtr)buffer[0], msg.lParam);
			return true;
		}
		}
		return result;
	}

	public int ToKeycode(int key)
	{
		int num = 0;
		if (nonchar_vkey_key[key] > 0)
		{
			num = XKeysymToKeycode(display, nonchar_vkey_key[key]);
		}
		if (num == 0)
		{
			num = XKeysymToKeycode(display, key);
		}
		return num;
	}

	public int ToUnicode(int vkey, int scan, out string buffer)
	{
		if (((uint)scan & 0x8000u) != 0)
		{
			buffer = string.Empty;
			return 0;
		}
		XEvent xevent = default(XEvent);
		xevent.AnyEvent.type = XEventName.KeyPress;
		xevent.KeyEvent.display = display;
		xevent.KeyEvent.keycode = 0;
		xevent.KeyEvent.state = 0;
		if ((key_state_table[16] & 0x80u) != 0)
		{
			xevent.KeyEvent.state |= 1;
		}
		if (((uint)key_state_table[20] & (true ? 1u : 0u)) != 0)
		{
			xevent.KeyEvent.state |= 2;
		}
		if ((key_state_table[17] & 0x80u) != 0)
		{
			xevent.KeyEvent.state |= 4;
		}
		if (((uint)key_state_table[144] & (true ? 1u : 0u)) != 0)
		{
			xevent.KeyEvent.state |= NumLockMask;
		}
		xevent.KeyEvent.state |= AltGrMask;
		for (int i = min_keycode; i <= max_keycode; i++)
		{
			if (xevent.KeyEvent.keycode != 0)
			{
				break;
			}
			if ((keyc2vkey[i] & 0xFF) == vkey)
			{
				xevent.KeyEvent.keycode = i;
				if ((EventToVkey(xevent) & 0xFF) != vkey)
				{
					xevent.KeyEvent.keycode = 0;
				}
			}
		}
		if (vkey >= 96 && vkey <= 105)
		{
			xevent.KeyEvent.keycode = XKeysymToKeycode(display, vkey - 96 + 65456);
		}
		if (vkey == 110)
		{
			xevent.KeyEvent.keycode = XKeysymToKeycode(display, 65454);
		}
		if (vkey == 108)
		{
			xevent.KeyEvent.keycode = XKeysymToKeycode(display, 65452);
		}
		if (xevent.KeyEvent.keycode == 0 && vkey != 252)
		{
			Console.Error.WriteLine("unknown virtual key {0:X}", vkey);
			buffer = string.Empty;
			return vkey;
		}
		XKeySym keysym;
		IntPtr status;
		int num = LookupString(ref xevent, 24, out keysym, out status);
		int num2 = (int)keysym;
		buffer = string.Empty;
		if (num == 0)
		{
			int num3 = MapDeadKeySym(num2);
			if (num3 != 0)
			{
				byte[] bytes = new byte[1] { (byte)num3 };
				Encoding encoding = Encoding.GetEncoding(new CultureInfo(lcid).TextInfo.ANSICodePage);
				buffer = new string(encoding.GetChars(bytes));
				num = -1;
			}
		}
		else
		{
			if ((xevent.KeyEvent.state & NumLockMask) == 0 && ((uint)xevent.KeyEvent.state & (true ? 1u : 0u)) != 0 && num2 >= 65456 && num2 <= 65465)
			{
				buffer = string.Empty;
				num = 0;
			}
			if (((uint)xevent.KeyEvent.state & 4u) != 0 && ((num2 >= 33 && num2 < 65) || (num2 > 90 && num2 < 97)))
			{
				buffer = string.Empty;
				num = 0;
			}
			if (num2 == 65535)
			{
				buffer = string.Empty;
				num = 0;
			}
			if (num2 == 65288 && (key_state_table[17] & 0x80u) != 0)
			{
				buffer = new string(new char[1] { '\u007f' });
				return 1;
			}
			switch (num2)
			{
			case 65288:
				buffer = new string(new char[1] { '\b' });
				return 1;
			case 65293:
				buffer = new string(new char[1] { '\r' });
				return 1;
			}
			if (num != 0)
			{
				buffer = lookup_buffer.ToString();
				num = buffer.Length;
			}
		}
		return num;
	}

	internal string GetCompositionString()
	{
		return stored_keyevent_string;
	}

	private MSG SendImeComposition(string s)
	{
		MSG result = default(MSG);
		result.message = Msg.WM_IME_COMPOSITION;
		result.refobject = s;
		stored_keyevent_string = s;
		return result;
	}

	private MSG SendKeyboardInput(VirtualKeys vkey, int scan, int keycode, KeybdEventFlags dw_flags, int time)
	{
		Msg message;
		if ((dw_flags & KeybdEventFlags.KeyUp) != 0)
		{
			bool flag = (key_state_table[18] & 0x80u) != 0 && (key_state_table[17] & 0x80) == 0;
			key_state_table[(int)vkey] &= 127;
			message = ((!flag) ? Msg.WM_KEYUP : Msg.WM_SYSKEYUP);
		}
		else
		{
			if ((key_state_table[(int)vkey] & 0x80) == 0)
			{
				key_state_table[(int)vkey] ^= 1;
			}
			key_state_table[(int)vkey] |= 128;
			message = (((key_state_table[18] & 0x80) == 0 || (key_state_table[17] & 0x80u) != 0) ? Msg.WM_KEYDOWN : Msg.WM_SYSKEYDOWN);
		}
		MSG mSG = default(MSG);
		mSG.message = message;
		mSG.wParam = (IntPtr)(int)vkey;
		mSG.lParam = GenerateLParam(mSG, keycode);
		return mSG;
	}

	private IntPtr GenerateLParam(MSG m, int keyCode)
	{
		byte b = 0;
		if (m.message == Msg.WM_SYSKEYUP || m.message == Msg.WM_KEYUP)
		{
			b = (byte)(b | 0x80u);
		}
		b = (byte)(b | 0x40u);
		if ((key_state_table[165] & 0x80u) != 0 || (key_state_table[164] & 0x80u) != 0 || (key_state_table[18] & 0x80u) != 0)
		{
			b = (byte)(b | 0x20u);
		}
		if ((key_state_table[45] & 0x80u) != 0 || (key_state_table[46] & 0x80u) != 0 || (key_state_table[36] & 0x80u) != 0 || (key_state_table[35] & 0x80u) != 0 || (key_state_table[38] & 0x80u) != 0 || (key_state_table[40] & 0x80u) != 0 || (key_state_table[37] & 0x80u) != 0 || (key_state_table[39] & 0x80u) != 0 || (key_state_table[17] & 0x80u) != 0 || (key_state_table[18] & 0x80u) != 0 || (key_state_table[144] & 0x80u) != 0 || (key_state_table[42] & 0x80u) != 0 || (key_state_table[13] & 0x80u) != 0 || (key_state_table[111] & 0x80u) != 0 || (key_state_table[33] & 0x80u) != 0 || (key_state_table[34] & 0x80u) != 0)
		{
			b = (byte)(b | 1u);
		}
		int num = (b & 0xFF) << 24;
		num |= (keyCode & 0xFF) << 16;
		num |= 1;
		return (IntPtr)num;
	}

	private void GenerateMessage(VirtualKeys vkey, int scan, int key_code, XEventName type, int event_time)
	{
		if ((vkey != VirtualKeys.VK_NUMLOCK) ? cap_state : num_state)
		{
			SetState(vkey, state: false);
			return;
		}
		KeybdEventFlags dw_flags = ((vkey == VirtualKeys.VK_NUMLOCK) ? KeybdEventFlags.ExtendedKey : KeybdEventFlags.None);
		KeybdEventFlags dw_flags2 = (KeybdEventFlags)(((vkey == VirtualKeys.VK_NUMLOCK) ? 1 : 0) | 2);
		if (((uint)key_state_table[(int)vkey] & (true ? 1u : 0u)) != 0)
		{
			if (type != XEventName.KeyPress)
			{
				SendKeyboardInput(vkey, scan, key_code, dw_flags, event_time);
				SendKeyboardInput(vkey, scan, key_code, dw_flags2, event_time);
				SetState(vkey, state: false);
				key_state_table[(int)vkey] &= 254;
			}
		}
		else if (type == XEventName.KeyPress)
		{
			SendKeyboardInput(vkey, scan, key_code, dw_flags, event_time);
			SendKeyboardInput(vkey, scan, key_code, dw_flags2, event_time);
			SetState(vkey, state: true);
			key_state_table[(int)vkey] |= 1;
		}
	}

	private void UpdateKeyState(XEvent xevent)
	{
		int num = EventToVkey(xevent);
		switch (xevent.type)
		{
		case XEventName.KeyRelease:
			key_state_table[num & 0xFF] &= 127;
			break;
		case XEventName.KeyPress:
			if ((key_state_table[num & 0xFF] & 0x80) == 0)
			{
				key_state_table[num & 0xFF] ^= 1;
			}
			key_state_table[num & 0xFF] |= 128;
			break;
		}
	}

	private void SetState(VirtualKeys key, bool state)
	{
		if (key == VirtualKeys.VK_NUMLOCK)
		{
			num_state = state;
		}
		else
		{
			cap_state = state;
		}
	}

	public int EventToVkey(XEvent e)
	{
		LookupString(ref e, 0, out var keysym, out var _);
		int num = (int)keysym;
		if ((e.KeyEvent.state & NumLockMask) != 0)
		{
			switch (num)
			{
			case 65452:
			case 65454:
			case 65456:
			case 65457:
			case 65458:
			case 65459:
			case 65460:
			case 65461:
			case 65462:
			case 65463:
			case 65464:
			case 65465:
				return nonchar_key_vkey[num & 0xFF];
			}
		}
		return keyc2vkey[e.KeyEvent.keycode];
	}

	private void CreateConversionArray(KeyboardLayouts layouts, KeyboardLayout layout)
	{
		XEvent xevent = default(XEvent);
		uint num = 0u;
		int[] array = new int[4];
		xevent.KeyEvent.display = display;
		xevent.KeyEvent.state = 0;
		for (int i = min_keycode; i <= max_keycode; i++)
		{
			int num2 = 0;
			int num3 = 0;
			xevent.KeyEvent.keycode = i;
			LookupString(ref xevent, 0, out var keysym, out var _);
			num = (uint)keysym;
			if (num != 0)
			{
				if (num >> 8 == 255)
				{
					num2 = nonchar_key_vkey[num & 0xFF];
					num3 = nonchar_key_scan[num & 0xFF];
					if (((uint)num3 & 0x100u) != 0)
					{
						num2 |= 0x100;
					}
				}
				else if (num == 32)
				{
					num2 = 32;
					num3 = 57;
				}
				else
				{
					int num4 = 0;
					int num5 = -1;
					for (int j = 0; j < syms; j++)
					{
						num = XKeycodeToKeysym(display, i, j);
						if (num < 2048 && num != 32)
						{
							array[j] = (sbyte)(num & 0xFF);
						}
						else
						{
							array[j] = (sbyte)MapDeadKeySym((int)num);
						}
					}
					for (int k = 0; k < layout.Keys.Length; k++)
					{
						int num6 = Math.Min(layout.Keys[k].Length, 4);
						int num7 = -1;
						int num8 = 0;
						while (num7 != 0 && num8 < num6)
						{
							sbyte b = (sbyte)layout.Keys[k][num8];
							if (b != array[num8])
							{
								num7 = 0;
							}
							if (num7 != 0 || num8 > num4)
							{
								num4 = num8;
								num5 = k;
							}
							if (num7 != 0)
							{
								break;
							}
							num8++;
						}
					}
					if (num5 >= 0)
					{
						if (num5 < layouts.scan_table[(int)layout.ScanIndex].Length)
						{
							num3 = layouts.scan_table[(int)layout.ScanIndex][num5];
						}
						if (num5 < layouts.vkey_table[(int)layout.VKeyIndex].Length)
						{
							num2 = layouts.vkey_table[(int)layout.VKeyIndex][num5];
						}
					}
				}
			}
			keyc2vkey[xevent.KeyEvent.keycode] = num2;
			keyc2scan[xevent.KeyEvent.keycode] = num3;
		}
	}

	private KeyboardLayout DetectLayout(KeyboardLayouts layouts)
	{
		XDisplayKeycodes(display, out min_keycode, out max_keycode);
		IntPtr data = XGetKeyboardMapping(display, (byte)min_keycode, max_keycode + 1 - min_keycode, out keysyms_per_keycode);
		lock (XlibLock)
		{
			XplatUIX11.XFree(data);
		}
		syms = keysyms_per_keycode;
		if (syms > 4)
		{
			syms = 2;
		}
		XModifierKeymap xModifierKeymap = default(XModifierKeymap);
		IntPtr intPtr = XGetModifierMapping(display);
		xModifierKeymap = (XModifierKeymap)Marshal.PtrToStructure(intPtr, typeof(XModifierKeymap));
		int num = 0;
		for (int i = 0; i < 8; i++)
		{
			int num2 = 0;
			while (num2 < xModifierKeymap.max_keypermod)
			{
				byte b = Marshal.ReadByte(xModifierKeymap.modifiermap, num);
				if (b != 0)
				{
					for (int j = 0; j < keysyms_per_keycode; j++)
					{
						if (XKeycodeToKeysym(display, b, j) == 65407)
						{
							NumLockMask = 1 << i;
						}
					}
				}
				num2++;
				num++;
			}
		}
		XFreeModifiermap(intPtr);
		int[] array = new int[4];
		KeyboardLayout keyboardLayout = null;
		int num3 = 0;
		int num4 = 0;
		KeyboardLayout[] layouts2 = layouts.Layouts;
		foreach (KeyboardLayout keyboardLayout2 in layouts2)
		{
			int num5 = 0;
			int num6 = 0;
			int num7 = 0;
			int num8 = 0;
			int num9 = 0;
			int num10 = -1;
			int num11 = min_keycode;
			for (int l = min_keycode; l <= max_keycode; l++)
			{
				for (int m = 0; m < syms; m++)
				{
					uint num12 = XKeycodeToKeysym(display, l, m);
					if (num12 < 2048 && num12 != 32)
					{
						array[m] = (sbyte)(num12 & 0xFF);
					}
					else
					{
						array[m] = (sbyte)MapDeadKeySym((int)num12);
					}
				}
				if (array[0] == 0)
				{
					continue;
				}
				for (num11 = 0; num11 < keyboardLayout2.Keys.Length; num11++)
				{
					int num13 = Math.Min(syms, keyboardLayout2.Keys[num11].Length);
					num5 = 0;
					int m = 0;
					while (num5 >= 0 && m < num13)
					{
						sbyte b2 = (sbyte)keyboardLayout2.Keys[num11][m];
						if (b2 != 0 && b2 == array[m])
						{
							num5++;
						}
						if (b2 != 0 && b2 != array[m])
						{
							num5 = -1;
						}
						m++;
					}
					if (num5 > 0)
					{
						num6 += num5;
						break;
					}
				}
				if (num5 > 0)
				{
					num7++;
					if (num11 > num10)
					{
						num9++;
					}
					num10 = num11;
				}
				else
				{
					num8++;
					num6 -= syms;
				}
			}
			if (num6 > num3 || (num6 == num3 && num9 > num4))
			{
				keyboardLayout = keyboardLayout2;
				num3 = num6;
				num4 = num9;
			}
		}
		if (keyboardLayout != null)
		{
			return keyboardLayout;
		}
		Console.WriteLine(Locale.GetText("Keyboard layout not recognized, using default layout: " + layouts.Layouts[0].Name));
		return layouts.Layouts[0];
	}

	private int MapDeadKeySym(int val)
	{
		switch (val)
		{
		case 65107:
		case 268500606:
			return 126;
		case 65105:
		case 268500519:
			return 180;
		case 65106:
		case 268500574:
			return 94;
		case 65104:
		case 268500576:
			return 96;
		case 65111:
		case 268500514:
			return 168;
		case 65115:
			return 184;
		case 65108:
			return 45;
		case 65109:
			return 162;
		case 65110:
			return 255;
		case 65112:
			return 48;
		case 65113:
			return 189;
		case 65114:
			return 183;
		case 65116:
			return 178;
		default:
			return 0;
		}
	}

	private XIMProperties[] GetSupportedInputStyles(IntPtr xim)
	{
		IntPtr value;
		string text = XGetIMValues(xim, "queryInputStyle", out value, IntPtr.Zero);
		if (text != null || value == IntPtr.Zero)
		{
			return new XIMProperties[0];
		}
		XIMStyles xIMStyles = (XIMStyles)Marshal.PtrToStructure(value, typeof(XIMStyles));
		XIMProperties[] array = new XIMProperties[xIMStyles.count_styles];
		for (int i = 0; i < xIMStyles.count_styles; i++)
		{
			array[i] = (XIMProperties)(int)Marshal.PtrToStructure(new IntPtr((long)xIMStyles.supported_styles + i * Marshal.SizeOf(typeof(IntPtr))), typeof(XIMProperties));
		}
		lock (XlibLock)
		{
			XplatUIX11.XFree(value);
			return array;
		}
	}

	private XIMProperties[] GetPreferredStyles()
	{
		string text = Environment.GetEnvironmentVariable("MONO_WINFORMS_XIM_STYLE");
		if (text == null)
		{
			text = "over-the-spot";
		}
		string[] array = text.Split(' ');
		XIMProperties[] array2 = new XIMProperties[array.Length];
		for (int i = 0; i < array.Length; i++)
		{
			switch (array[i])
			{
			case "over-the-spot":
				array2[i] = XIMProperties.XIMPreeditPosition | XIMProperties.XIMStatusNothing;
				break;
			case "on-the-spot":
				array2[i] = XIMProperties.XIMPreeditCallbacks | XIMProperties.XIMStatusNothing;
				break;
			case "root":
				array2[i] = XIMProperties.XIMPreeditNothing | XIMProperties.XIMStatusNothing;
				break;
			}
		}
		return array2;
	}

	private IEnumerable GetMatchingStylesInPreferredOrder(IntPtr xim)
	{
		XIMProperties[] supportedStyles = GetSupportedInputStyles(xim);
		XIMProperties[] preferredStyles = GetPreferredStyles();
		foreach (XIMProperties p in preferredStyles)
		{
			if (Array.IndexOf(supportedStyles, p) >= 0)
			{
				yield return p;
			}
		}
	}

	private IntPtr CreateXic(IntPtr window, IntPtr xim)
	{
		IntPtr intPtr = IntPtr.Zero;
		IEnumerator enumerator = GetMatchingStylesInPreferredOrder(xim).GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				switch (ximStyle = (XIMProperties)(int)enumerator.Current)
				{
				case XIMProperties.XIMPreeditPosition | XIMProperties.XIMStatusNothing:
					intPtr = CreateOverTheSpotXic(window, xim);
					if (!(intPtr != IntPtr.Zero))
					{
					}
					break;
				case XIMProperties.XIMPreeditCallbacks | XIMProperties.XIMStatusNothing:
					intPtr = CreateOnTheSpotXic(window, xim);
					if (!(intPtr != IntPtr.Zero))
					{
					}
					break;
				case XIMProperties.XIMPreeditNothing | XIMProperties.XIMStatusNothing:
					intPtr = XCreateIC(xim, "inputStyle", XIMProperties.XIMPreeditNothing | XIMProperties.XIMStatusNothing, "clientWindow", window, IntPtr.Zero);
					break;
				}
			}
		}
		finally
		{
			IDisposable disposable = enumerator as IDisposable;
			if (disposable != null)
			{
				disposable.Dispose();
			}
		}
		if (intPtr == IntPtr.Zero)
		{
			ximStyle = XIMProperties.XIMPreeditNothing | XIMProperties.XIMStatusNothing;
			intPtr = XCreateIC(xim, "inputStyle", XIMProperties.XIMPreeditNothing | XIMProperties.XIMStatusNothing, "clientWindow", window, IntPtr.Zero);
		}
		return intPtr;
	}

	private IntPtr CreateOverTheSpotXic(IntPtr window, IntPtr xim)
	{
		Control control = Control.FromHandle(window);
		string name = $"-*-*-*-*-*-*-{(int)control.Font.Size}-*-*-*-*-*-*-*";
		IntPtr list;
		int count;
		IntPtr value = XCreateFontSet(display, name, out list, out count, IntPtr.Zero);
		XPoint xPoint = new XPoint();
		xPoint.X = 0;
		xPoint.Y = 0;
		IntPtr intPtr = IntPtr.Zero;
		IntPtr intPtr2 = IntPtr.Zero;
		try
		{
			intPtr = Marshal.StringToHGlobalAnsi("spotLocation");
			intPtr2 = Marshal.StringToHGlobalAnsi("fontSet");
			IntPtr value2 = XVaCreateNestedList(0, intPtr, xPoint, intPtr2, value, IntPtr.Zero);
			return XCreateIC(xim, "inputStyle", XIMProperties.XIMPreeditPosition | XIMProperties.XIMStatusNothing, "clientWindow", window, "preeditAttributes", value2, IntPtr.Zero);
		}
		finally
		{
			if (intPtr != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(intPtr);
			}
			if (intPtr2 != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(intPtr2);
			}
			XFreeStringList(list);
		}
	}

	private IntPtr CreateOnTheSpotXic(IntPtr window, IntPtr xim)
	{
		callbackContext = new XIMCallbackContext(window);
		return callbackContext.CreateXic(window, xim);
	}

	internal void SetCaretPos(CaretStruct caret, IntPtr handle, int x, int y)
	{
		if (ximStyle == (XIMProperties.XIMPreeditPosition | XIMProperties.XIMStatusNothing))
		{
			if (positionContext == null)
			{
				positionContext = new XIMPositionContext();
			}
			positionContext.Caret = caret;
			positionContext.X = x;
			positionContext.Y = y + caret.Height;
			MoveCurrentCaretPos();
		}
	}

	internal void MoveCurrentCaretPos()
	{
		if (positionContext == null || ximStyle != (XIMProperties.XIMPreeditPosition | XIMProperties.XIMStatusNothing) || client_window == IntPtr.Zero)
		{
			return;
		}
		int x = positionContext.X;
		int y = positionContext.Y;
		CaretStruct caret = positionContext.Caret;
		IntPtr xic = GetXic(client_window);
		if (xic == IntPtr.Zero)
		{
			return;
		}
		Control control = Control.FromHandle(client_window);
		if (control == null || !control.IsHandleCreated)
		{
			return;
		}
		control = Control.FromHandle(caret.Hwnd);
		if (control == null || !control.IsHandleCreated)
		{
			return;
		}
		Hwnd hwnd = Hwnd.ObjectFromHandle(client_window);
		if (!hwnd.mapped)
		{
			return;
		}
		int intdest_x_return;
		int dest_y_return;
		lock (XlibLock)
		{
			XplatUIX11.XTranslateCoordinates(display, client_window, client_window, x, y, out intdest_x_return, out dest_y_return, out var _);
		}
		XPoint xPoint = new XPoint();
		xPoint.X = (short)intdest_x_return;
		xPoint.Y = (short)dest_y_return;
		IntPtr intPtr = IntPtr.Zero;
		try
		{
			intPtr = Marshal.StringToHGlobalAnsi("spotLocation");
			IntPtr value = XVaCreateNestedList(0, intPtr, xPoint, IntPtr.Zero);
			XSetICValues(xic, "preeditAttributes", value, IntPtr.Zero);
		}
		finally
		{
			if (intPtr != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(intPtr);
			}
		}
	}

	private int LookupString(ref XEvent xevent, int len, out XKeySym keysym, out IntPtr status)
	{
		status = IntPtr.Zero;
		IntPtr xic = GetXic(client_window);
		IntPtr keysym2;
		int count;
		if (xic != IntPtr.Zero)
		{
			while (true)
			{
				count = Xutf8LookupString(xic, ref xevent, utf8_buffer, 100, out keysym2, out status);
				if ((int)status != -1)
				{
					break;
				}
				utf8_buffer = new byte[utf8_buffer.Length << 1];
			}
			lookup_buffer.Length = 0;
			string @string = Encoding.UTF8.GetString(utf8_buffer, 0, count);
			lookup_buffer.Append(@string);
			keysym = (XKeySym)keysym2.ToInt32();
			return @string.Length;
		}
		lookup_buffer.Length = 0;
		count = XLookupString(ref xevent, lookup_buffer, len, out keysym2, IntPtr.Zero);
		keysym = (XKeySym)keysym2.ToInt32();
		return count;
	}

	[DllImport("libX11")]
	private static extern IntPtr XOpenIM(IntPtr display, IntPtr rdb, IntPtr res_name, IntPtr res_class);

	[DllImport("libX11", CallingConvention = CallingConvention.Cdecl)]
	private static extern IntPtr XCreateIC(IntPtr xim, string name, XIMProperties im_style, string name2, IntPtr value2, IntPtr terminator);

	[DllImport("libX11", CallingConvention = CallingConvention.Cdecl)]
	private static extern IntPtr XCreateIC(IntPtr xim, string name, XIMProperties im_style, string name2, IntPtr value2, string name3, IntPtr value3, IntPtr terminator);

	[DllImport("libX11", CallingConvention = CallingConvention.Cdecl)]
	private static extern IntPtr XVaCreateNestedList(int dummy, IntPtr name0, XPoint value0, IntPtr terminator);

	[DllImport("libX11", CallingConvention = CallingConvention.Cdecl)]
	private static extern IntPtr XVaCreateNestedList(int dummy, IntPtr name0, XPoint value0, IntPtr name1, IntPtr value1, IntPtr terminator);

	[DllImport("libX11", CallingConvention = CallingConvention.Cdecl)]
	private static extern IntPtr XVaCreateNestedList(int dummy, IntPtr name0, IntPtr value0, IntPtr name1, IntPtr value1, IntPtr name2, IntPtr value2, IntPtr name3, IntPtr value3, IntPtr terminator);

	[DllImport("libX11")]
	private static extern IntPtr XCreateFontSet(IntPtr display, string name, out IntPtr list, out int count, IntPtr terminator);

	[DllImport("libX11")]
	internal static extern void XFreeFontSet(IntPtr data);

	[DllImport("libX11")]
	private static extern void XFreeStringList(IntPtr ptr);

	[DllImport("libX11")]
	private static extern void XCloseIM(IntPtr xim);

	[DllImport("libX11")]
	private static extern void XDestroyIC(IntPtr xic);

	[DllImport("libX11")]
	private static extern string XGetIMValues(IntPtr xim, string name, out IntPtr value, IntPtr terminator);

	[DllImport("libX11")]
	private static extern string XGetICValues(IntPtr xic, string name, out EventMask value, IntPtr terminator);

	[DllImport("libX11", CallingConvention = CallingConvention.Cdecl)]
	private static extern void XSetICValues(IntPtr xic, string name, IntPtr value, IntPtr terminator);

	[DllImport("libX11")]
	private static extern void XSetICFocus(IntPtr xic);

	[DllImport("libX11")]
	private static extern void XUnsetICFocus(IntPtr xic);

	[DllImport("libX11")]
	private static extern string Xutf8ResetIC(IntPtr xic);

	[DllImport("libX11")]
	private static extern bool XSupportsLocale();

	[DllImport("libX11")]
	private static extern bool XSetLocaleModifiers(string mods);

	[DllImport("libX11")]
	internal static extern int XLookupString(ref XEvent xevent, StringBuilder buffer, int num_bytes, out IntPtr keysym, IntPtr status);

	[DllImport("libX11")]
	internal static extern int Xutf8LookupString(IntPtr xic, ref XEvent xevent, byte[] buffer, int num_bytes, out IntPtr keysym, out IntPtr status);

	[DllImport("libX11")]
	private static extern IntPtr XGetKeyboardMapping(IntPtr display, byte first_keycode, int keycode_count, out int keysyms_per_keycode_return);

	[DllImport("libX11")]
	private static extern void XDisplayKeycodes(IntPtr display, out int min, out int max);

	[DllImport("libX11")]
	private static extern uint XKeycodeToKeysym(IntPtr display, int keycode, int index);

	[DllImport("libX11")]
	private static extern int XKeysymToKeycode(IntPtr display, IntPtr keysym);

	private static int XKeysymToKeycode(IntPtr display, int keysym)
	{
		return XKeysymToKeycode(display, (IntPtr)keysym);
	}

	[DllImport("libX11")]
	internal static extern IntPtr XGetModifierMapping(IntPtr display);

	[DllImport("libX11")]
	internal static extern int XFreeModifiermap(IntPtr modmap);
}
