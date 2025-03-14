using System.Collections;
using System.Text;

namespace System.Windows.Forms;

public class SendKeys
{
	private struct Keyword
	{
		internal string keyword;

		internal int vk;

		public Keyword(string keyword, int vk)
		{
			this.keyword = keyword;
			this.vk = vk;
		}
	}

	private static Queue keys;

	private static Hashtable keywords;

	private static object lockobj;

	private SendKeys()
	{
	}

	static SendKeys()
	{
		keys = new Queue();
		lockobj = new object();
		keywords = new Hashtable();
		keywords.Add("BACKSPACE", 8);
		keywords.Add("BS", 8);
		keywords.Add("BKSP", 8);
		keywords.Add("BREAK", 3);
		keywords.Add("CAPSLOCK", 20);
		keywords.Add("DELETE", 46);
		keywords.Add("DEL", 46);
		keywords.Add("DOWN", 40);
		keywords.Add("END", 35);
		keywords.Add("ENTER", 13);
		keywords.Add("~", 13);
		keywords.Add("ESC", 27);
		keywords.Add("HELP", 47);
		keywords.Add("HOME", 36);
		keywords.Add("INSERT", 45);
		keywords.Add("INS", 45);
		keywords.Add("LEFT", 37);
		keywords.Add("NUMLOCK", 144);
		keywords.Add("PGDN", 34);
		keywords.Add("PGUP", 33);
		keywords.Add("PRTSC", 44);
		keywords.Add("RIGHT", 39);
		keywords.Add("SCROLLLOCK", 145);
		keywords.Add("TAB", 9);
		keywords.Add("UP", 38);
		keywords.Add("F1", 112);
		keywords.Add("F2", 113);
		keywords.Add("F3", 114);
		keywords.Add("F4", 115);
		keywords.Add("F5", 116);
		keywords.Add("F6", 117);
		keywords.Add("F7", 118);
		keywords.Add("F8", 119);
		keywords.Add("F9", 120);
		keywords.Add("F10", 121);
		keywords.Add("F11", 122);
		keywords.Add("F12", 123);
		keywords.Add("F13", 124);
		keywords.Add("F14", 125);
		keywords.Add("F15", 126);
		keywords.Add("F16", 127);
		keywords.Add("ADD", 107);
		keywords.Add("SUBTRACT", 109);
		keywords.Add("MULTIPLY", 106);
		keywords.Add("DIVIDE", 111);
		keywords.Add("+", 16);
		keywords.Add("^", 17);
		keywords.Add("%", 18);
	}

	private static void AddVKey(int vk, bool down)
	{
		MSG mSG = default(MSG);
		mSG.message = ((!down) ? Msg.WM_KEYUP : Msg.WM_KEYDOWN);
		mSG.wParam = new IntPtr(vk);
		mSG.lParam = IntPtr.Zero;
		keys.Enqueue(mSG);
	}

	private static void AddVKey(int vk, int repeat_count)
	{
		for (int i = 0; i < repeat_count; i++)
		{
			MSG mSG = default(MSG);
			mSG.message = Msg.WM_KEYDOWN;
			mSG.wParam = new IntPtr(vk);
			mSG.lParam = (IntPtr)1;
			keys.Enqueue(mSG);
			mSG = default(MSG);
			mSG.message = Msg.WM_KEYUP;
			mSG.wParam = new IntPtr(vk);
			mSG.lParam = IntPtr.Zero;
			keys.Enqueue(mSG);
		}
	}

	private static void AddKey(char key, int repeat_count)
	{
		for (int i = 0; i < repeat_count; i++)
		{
			MSG mSG = default(MSG);
			mSG.message = Msg.WM_KEYDOWN;
			mSG.wParam = new IntPtr(key);
			mSG.lParam = IntPtr.Zero;
			keys.Enqueue(mSG);
			mSG = default(MSG);
			mSG.message = Msg.WM_KEYUP;
			mSG.wParam = new IntPtr(key);
			mSG.lParam = IntPtr.Zero;
			keys.Enqueue(mSG);
		}
	}

	private static void Parse(string key_string)
	{
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		bool flag4 = false;
		bool flag5 = false;
		bool flag6 = false;
		StringBuilder stringBuilder = new StringBuilder();
		StringBuilder stringBuilder2 = new StringBuilder();
		int length = key_string.Length;
		for (int i = 0; i < length; i++)
		{
			switch (key_string[i])
			{
			case '{':
			{
				stringBuilder2.Remove(0, stringBuilder2.Length);
				stringBuilder.Remove(0, stringBuilder.Length);
				int j;
				for (j = i + 1; j < length && key_string[j] != '}'; j++)
				{
					if (char.IsWhiteSpace(key_string[j]))
					{
						if (flag3)
						{
							throw new ArgumentException("SendKeys string {0} is not valid.", key_string);
						}
						flag3 = true;
					}
					else if (flag3)
					{
						if (!char.IsDigit(key_string[j]))
						{
							throw new ArgumentException("SendKeys string {0} is not valid.", key_string);
						}
						stringBuilder.Append(key_string[j]);
					}
					else
					{
						stringBuilder2.Append(key_string[j]);
					}
				}
				if (j == length || j == i + 1)
				{
					throw new ArgumentException("SendKeys string {0} is not valid.", key_string);
				}
				if (keywords.Contains(stringBuilder2.ToString().ToUpper()))
				{
					flag2 = true;
					int num = 1;
					if (stringBuilder.Length > 0)
					{
						num = int.Parse(stringBuilder.ToString());
					}
					if (flag2)
					{
						AddVKey((int)keywords[stringBuilder2.ToString().ToUpper()], (stringBuilder.Length == 0) ? 1 : num);
					}
					else if (char.IsUpper(char.Parse(stringBuilder2.ToString())))
					{
						if (!flag4)
						{
							AddVKey((int)keywords["+"], down: true);
						}
						AddKey(char.Parse(stringBuilder2.ToString()), 1);
						if (!flag4)
						{
							AddVKey((int)keywords["+"], down: false);
						}
					}
					else
					{
						AddKey(char.Parse(stringBuilder2.ToString().ToUpper()), (stringBuilder.Length == 0) ? 1 : num);
					}
					i = j;
					flag3 = (flag2 = false);
					if (flag4)
					{
						AddVKey((int)keywords["+"], down: false);
					}
					if (flag5)
					{
						AddVKey((int)keywords["^"], down: false);
					}
					if (flag6)
					{
						AddVKey((int)keywords["%"], down: false);
					}
					flag4 = (flag5 = (flag6 = false));
					continue;
				}
				throw new ArgumentException("SendKeys string {0} is not valid.", key_string);
			}
			case '+':
				AddVKey((int)keywords["+"], down: true);
				flag4 = true;
				continue;
			case '^':
				AddVKey((int)keywords["^"], down: true);
				flag5 = true;
				continue;
			case '%':
				AddVKey((int)keywords["%"], down: true);
				flag6 = true;
				continue;
			case '~':
				AddVKey((int)keywords["ENTER"], 1);
				continue;
			case '(':
				flag = true;
				continue;
			case ')':
				if (flag4)
				{
					AddVKey((int)keywords["+"], down: false);
				}
				if (flag5)
				{
					AddVKey((int)keywords["^"], down: false);
				}
				if (flag6)
				{
					AddVKey((int)keywords["%"], down: false);
				}
				flag4 = (flag5 = (flag6 = (flag = false)));
				continue;
			}
			if (char.IsUpper(key_string[i]))
			{
				if (!flag4)
				{
					AddVKey((int)keywords["+"], down: true);
				}
				AddKey(key_string[i], 1);
				if (!flag4)
				{
					AddVKey((int)keywords["+"], down: false);
				}
			}
			else
			{
				AddKey(char.Parse(key_string[i].ToString().ToUpper()), 1);
			}
			if (!flag)
			{
				if (flag4)
				{
					AddVKey((int)keywords["+"], down: false);
				}
				if (flag5)
				{
					AddVKey((int)keywords["^"], down: false);
				}
				if (flag6)
				{
					AddVKey((int)keywords["%"], down: false);
				}
				flag4 = (flag5 = (flag6 = (flag = false)));
			}
		}
		if (flag)
		{
			throw new ArgumentException("SendKeys string {0} is not valid.", key_string);
		}
		if (flag4)
		{
			AddVKey((int)keywords["+"], down: false);
		}
		if (flag5)
		{
			AddVKey((int)keywords["^"], down: false);
		}
		if (flag6)
		{
			AddVKey((int)keywords["%"], down: false);
		}
	}

	private static void SendInput()
	{
		IntPtr intPtr = XplatUI.GetActive();
		if (intPtr != IntPtr.Zero)
		{
			Form form = (Form)Control.FromHandle(intPtr);
			if (form != null && form.ActiveControl != null)
			{
				intPtr = form.ActiveControl.Handle;
			}
			else if (form != null)
			{
				intPtr = form.Handle;
			}
		}
		XplatUI.SendInput(intPtr, keys);
		keys.Clear();
	}

	public static void Flush()
	{
		Application.DoEvents();
	}

	public static void Send(string keys)
	{
		Parse(keys);
		SendInput();
	}

	public static void SendWait(string keys)
	{
		lock (lockobj)
		{
			Send(keys);
		}
		Flush();
	}
}
