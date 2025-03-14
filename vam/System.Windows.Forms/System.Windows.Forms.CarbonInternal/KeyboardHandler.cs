using System.Runtime.InteropServices;
using System.Text;

namespace System.Windows.Forms.CarbonInternal;

internal class KeyboardHandler : EventHandlerBase, IEventHandler
{
	internal const uint kEventRawKeyDown = 1u;

	internal const uint kEventRawKeyRepeat = 2u;

	internal const uint kEventRawKeyUp = 3u;

	internal const uint kEventRawKeyModifiersChanged = 4u;

	internal const uint kEventHotKeyPressed = 5u;

	internal const uint kEventHotKeyReleased = 6u;

	internal const uint kEventParamKeyMacCharCodes = 1801676914u;

	internal const uint kEventParamKeyCode = 1801678692u;

	internal const uint kEventParamKeyModifiers = 1802334052u;

	internal const uint kEventTextInputUnicodeForKeyEvent = 2u;

	internal const uint kEventParamTextInputSendText = 1953723512u;

	internal const uint typeChar = 1413830740u;

	internal const uint typeUInt32 = 1835100014u;

	internal const uint typeUnicodeText = 1970567284u;

	internal static byte[] key_filter_table;

	internal static byte[] key_modifier_table;

	internal static byte[] key_translation_table;

	internal static byte[] char_translation_table;

	internal static bool translate_modifier;

	internal string ComposedString;

	internal Keys ModifierKeys
	{
		get
		{
			Keys keys = Keys.None;
			if (key_modifier_table[9] == 1 || key_modifier_table[13] == 1)
			{
				keys |= Keys.Shift;
			}
			if (key_modifier_table[8] == 1)
			{
				keys |= Keys.Alt;
			}
			if (key_modifier_table[12] == 1 || key_modifier_table[14] == 1)
			{
				keys |= Keys.Control;
			}
			return keys;
		}
	}

	internal KeyboardHandler(XplatUICarbon driver)
		: base(driver)
	{
	}

	static KeyboardHandler()
	{
		byte[] array = new byte[256];
		array[16] = 1;
		array[28] = 1;
		array[29] = 1;
		array[30] = 1;
		array[31] = 1;
		key_filter_table = array;
		char_translation_table = new byte[256]
		{
			0, 1, 2, 3, 4, 5, 6, 7, 8, 9,
			10, 11, 12, 13, 14, 15, 16, 17, 18, 19,
			20, 21, 22, 23, 24, 25, 26, 27, 37, 39,
			38, 40, 32, 49, 34, 51, 52, 53, 55, 222,
			57, 48, 56, 187, 188, 189, 190, 191, 48, 49,
			50, 51, 52, 53, 54, 55, 56, 57, 58, 186,
			60, 61, 62, 63, 50, 65, 66, 67, 68, 187,
			70, 71, 72, 73, 74, 75, 76, 77, 78, 79,
			80, 81, 82, 83, 84, 85, 86, 87, 88, 89,
			90, 219, 220, 221, 54, 189, 192, 65, 66, 67,
			68, 69, 70, 71, 72, 73, 74, 75, 76, 77,
			78, 79, 80, 81, 82, 83, 84, 85, 86, 87,
			88, 89, 90, 123, 124, 125, 126, 46, 128, 129,
			130, 131, 132, 133, 134, 135, 136, 137, 138, 139,
			140, 141, 142, 143, 144, 145, 146, 147, 148, 149,
			150, 151, 152, 153, 154, 155, 156, 157, 158, 159,
			160, 161, 162, 163, 164, 165, 166, 167, 168, 169,
			170, 171, 172, 173, 174, 175, 176, 177, 178, 179,
			180, 181, 182, 183, 184, 185, 186, 187, 188, 189,
			190, 191, 192, 193, 194, 195, 196, 197, 198, 199,
			200, 201, 202, 203, 204, 205, 206, 207, 208, 209,
			210, 211, 212, 213, 214, 215, 216, 217, 218, 219,
			220, 221, 222, 223, 224, 225, 226, 227, 228, 229,
			230, 231, 232, 233, 234, 235, 236, 237, 238, 239,
			240, 241, 242, 243, 244, 245, 246, 247, 248, 249,
			250, 251, 252, 253, 254, 255
		};
		key_translation_table = new byte[256]
		{
			0, 1, 2, 3, 4, 5, 6, 7, 8, 9,
			10, 11, 12, 13, 14, 15, 16, 17, 18, 19,
			20, 21, 22, 23, 24, 25, 26, 27, 28, 29,
			30, 31, 32, 33, 34, 35, 36, 37, 38, 39,
			40, 41, 42, 43, 44, 45, 46, 47, 48, 49,
			50, 51, 52, 53, 54, 55, 56, 57, 58, 59,
			60, 61, 62, 63, 64, 65, 66, 67, 68, 69,
			70, 71, 72, 73, 74, 75, 76, 77, 78, 79,
			80, 81, 82, 83, 84, 85, 86, 87, 88, 89,
			90, 91, 92, 93, 94, 95, 116, 117, 118, 114,
			119, 120, 121, 103, 104, 105, 106, 107, 108, 109,
			122, 123, 112, 113, 114, 115, 116, 117, 115, 119,
			113, 121, 112, 123, 124, 125, 126, 127, 128, 129,
			130, 131, 132, 133, 134, 135, 136, 137, 138, 139,
			140, 141, 142, 143, 144, 145, 146, 147, 148, 149,
			150, 151, 152, 153, 154, 155, 156, 157, 158, 159,
			160, 161, 162, 163, 164, 165, 166, 167, 168, 169,
			170, 171, 172, 173, 174, 175, 176, 177, 178, 179,
			180, 181, 182, 183, 184, 185, 186, 187, 188, 189,
			190, 191, 192, 193, 194, 195, 196, 197, 198, 199,
			200, 201, 202, 203, 204, 205, 206, 207, 208, 209,
			210, 211, 212, 213, 214, 215, 216, 217, 218, 219,
			220, 221, 222, 223, 224, 225, 226, 227, 228, 229,
			230, 231, 232, 233, 234, 235, 236, 237, 238, 239,
			240, 241, 242, 243, 244, 245, 246, 247, 248, 249,
			250, 251, 252, 253, 254, 255
		};
		key_modifier_table = new byte[32];
	}

	private void ModifierToVirtualKey(int i, ref MSG msg, bool down)
	{
		msg.hwnd = XplatUICarbon.FocusWindow;
		switch (i)
		{
		case 9:
		case 13:
			msg.message = ((!down) ? Msg.WM_KEYUP : Msg.WM_KEYDOWN);
			msg.wParam = (IntPtr)16;
			msg.lParam = IntPtr.Zero;
			break;
		case 12:
		case 14:
			msg.message = ((!down) ? Msg.WM_KEYUP : Msg.WM_KEYDOWN);
			msg.wParam = (IntPtr)17;
			msg.lParam = IntPtr.Zero;
			break;
		case 8:
			msg.message = ((!down) ? Msg.WM_SYSKEYUP : Msg.WM_SYSKEYDOWN);
			msg.wParam = (IntPtr)18;
			msg.lParam = new IntPtr(536870912);
			break;
		}
	}

	public void ProcessModifiers(IntPtr eventref, ref MSG msg)
	{
		uint data = 0u;
		GetEventParameter(eventref, 1802334052u, 1835100014u, IntPtr.Zero, (uint)Marshal.SizeOf(typeof(uint)), IntPtr.Zero, ref data);
		for (int i = 0; i < 32; i++)
		{
			if (key_modifier_table[i] == 1 && (data & (1 << i)) == 0L)
			{
				ModifierToVirtualKey(i, ref msg, down: false);
				key_modifier_table[i] = 0;
				break;
			}
			if (key_modifier_table[i] == 0 && (data & (1 << i)) == 1 << i)
			{
				ModifierToVirtualKey(i, ref msg, down: true);
				key_modifier_table[i] = 1;
				break;
			}
		}
	}

	public void ProcessText(IntPtr eventref, ref MSG msg)
	{
		uint outsize = 0u;
		IntPtr zero = IntPtr.Zero;
		GetEventParameter(eventref, 1953723512u, 1970567284u, IntPtr.Zero, 0u, ref outsize, IntPtr.Zero);
		zero = Marshal.AllocHGlobal((int)outsize);
		byte[] array = new byte[outsize];
		GetEventParameter(eventref, 1953723512u, 1970567284u, IntPtr.Zero, outsize, IntPtr.Zero, zero);
		Marshal.Copy(zero, array, 0, (int)outsize);
		Marshal.FreeHGlobal(zero);
		if (key_filter_table[array[0]] == 0)
		{
			if (outsize == 1)
			{
				msg.message = Msg.WM_CHAR;
				msg.wParam = ((!BitConverter.IsLittleEndian) ? ((IntPtr)array[outsize - 1]) : ((IntPtr)array[0]));
				msg.lParam = IntPtr.Zero;
				msg.hwnd = XplatUICarbon.FocusWindow;
			}
			else
			{
				msg.message = Msg.WM_IME_COMPOSITION;
				Encoding encoding = ((!BitConverter.IsLittleEndian) ? Encoding.BigEndianUnicode : Encoding.Unicode);
				ComposedString = encoding.GetString(array);
				msg.hwnd = XplatUICarbon.FocusWindow;
			}
		}
	}

	public void ProcessKeyPress(IntPtr eventref, ref MSG msg)
	{
		byte data = 0;
		byte data2 = 0;
		GetEventParameter(eventref, 1801676914u, 1413830740u, IntPtr.Zero, (uint)Marshal.SizeOf(typeof(byte)), IntPtr.Zero, ref data);
		GetEventParameter(eventref, 1801678692u, 1835100014u, IntPtr.Zero, (uint)Marshal.SizeOf(typeof(byte)), IntPtr.Zero, ref data2);
		msg.lParam = (IntPtr)data;
		msg.wParam = ((data != 16) ? ((IntPtr)char_translation_table[data]) : ((IntPtr)key_translation_table[data2]));
		msg.hwnd = XplatUICarbon.FocusWindow;
	}

	public bool ProcessEvent(IntPtr callref, IntPtr eventref, IntPtr handle, uint kind, ref MSG msg)
	{
		uint eventClass = EventHandler.GetEventClass(eventref);
		bool result = true;
		switch (eventClass)
		{
		case 1952807028u:
			if (kind == 2)
			{
				ProcessText(eventref, ref msg);
			}
			else
			{
				Console.WriteLine("WARNING: KeyboardHandler.ProcessEvent default handler for kEventClassTextInput should not be reached");
			}
			break;
		case 1801812322u:
			switch (kind)
			{
			case 1u:
			case 2u:
				msg.message = Msg.WM_KEYDOWN;
				ProcessKeyPress(eventref, ref msg);
				break;
			case 3u:
				msg.message = Msg.WM_KEYUP;
				ProcessKeyPress(eventref, ref msg);
				break;
			case 4u:
				ProcessModifiers(eventref, ref msg);
				break;
			default:
				Console.WriteLine("WARNING: KeyboardHandler.ProcessEvent default handler for kEventClassKeyboard should not be reached");
				break;
			}
			break;
		default:
			Console.WriteLine("WARNING: KeyboardHandler.ProcessEvent default handler for kEventClassTextInput should not be reached");
			break;
		}
		return result;
	}

	public bool TranslateMessage(ref MSG msg)
	{
		bool result = false;
		if (msg.message >= Msg.WM_KEYDOWN && msg.message <= Msg.WM_KEYLAST)
		{
			result = true;
		}
		if (msg.message != Msg.WM_KEYDOWN && msg.message != Msg.WM_SYSKEYDOWN && msg.message != Msg.WM_KEYUP && msg.message != Msg.WM_SYSKEYUP && msg.message != Msg.WM_CHAR && msg.message != Msg.WM_SYSCHAR)
		{
			return result;
		}
		if (key_modifier_table[8] == 1 && key_modifier_table[12] == 0 && key_modifier_table[14] == 0)
		{
			if (msg.message == Msg.WM_KEYDOWN)
			{
				msg.message = Msg.WM_SYSKEYDOWN;
			}
			else if (msg.message == Msg.WM_CHAR)
			{
				msg.message = Msg.WM_SYSCHAR;
				translate_modifier = true;
			}
			else
			{
				if (msg.message != Msg.WM_KEYUP)
				{
					return result;
				}
				msg.message = Msg.WM_SYSKEYUP;
			}
			msg.lParam = new IntPtr(536870912);
		}
		else if (msg.message == Msg.WM_SYSKEYUP && translate_modifier && msg.wParam == (IntPtr)18)
		{
			msg.message = Msg.WM_KEYUP;
			msg.lParam = IntPtr.Zero;
			translate_modifier = false;
		}
		return result;
	}

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	private static extern int GetEventParameter(IntPtr eventref, uint name, uint type, IntPtr outtype, uint size, ref uint outsize, IntPtr data);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	private static extern int GetEventParameter(IntPtr eventref, uint name, uint type, IntPtr outtype, uint size, IntPtr outsize, IntPtr data);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	private static extern int GetEventParameter(IntPtr eventref, uint name, uint type, IntPtr outtype, uint size, IntPtr outsize, ref byte data);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	private static extern int GetEventParameter(IntPtr eventref, uint name, uint type, IntPtr outtype, uint size, IntPtr outsize, ref uint data);
}
