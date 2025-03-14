using System.Runtime.InteropServices;

namespace System.Windows.Forms;

public struct Message
{
	private int msg;

	private IntPtr hwnd;

	private IntPtr lParam;

	private IntPtr wParam;

	private IntPtr result;

	public IntPtr HWnd
	{
		get
		{
			return hwnd;
		}
		set
		{
			hwnd = value;
		}
	}

	public IntPtr LParam
	{
		get
		{
			return lParam;
		}
		set
		{
			lParam = value;
		}
	}

	public int Msg
	{
		get
		{
			return msg;
		}
		set
		{
			msg = value;
		}
	}

	public IntPtr Result
	{
		get
		{
			return result;
		}
		set
		{
			result = value;
		}
	}

	public IntPtr WParam
	{
		get
		{
			return wParam;
		}
		set
		{
			wParam = value;
		}
	}

	public static Message Create(IntPtr hWnd, int msg, IntPtr wparam, IntPtr lparam)
	{
		Message message = default(Message);
		message.msg = msg;
		message.hwnd = hWnd;
		message.wParam = wparam;
		message.lParam = lparam;
		return message;
	}

	public override bool Equals(object o)
	{
		if (!(o is Message))
		{
			return false;
		}
		return msg == ((Message)o).msg && hwnd == ((Message)o).hwnd && lParam == ((Message)o).lParam && wParam == ((Message)o).wParam && result == ((Message)o).result;
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}

	public object GetLParam(Type cls)
	{
		return Marshal.PtrToStructure(lParam, cls);
	}

	public override string ToString()
	{
		return $"msg=0x{msg:x} ({((Msg)msg).ToString()}) hwnd=0x{hwnd.ToInt32():x} wparam=0x{wParam.ToInt32():x} lparam=0x{lParam.ToInt32():x} result=0x{result.ToInt32():x}";
	}

	public static bool operator ==(Message a, Message b)
	{
		return a.hwnd == b.hwnd && a.lParam == b.lParam && a.msg == b.msg && a.result == b.result && a.wParam == b.wParam;
	}

	public static bool operator !=(Message a, Message b)
	{
		return !(a == b);
	}
}
