using System.Collections;

namespace System.Windows.Forms;

public class NativeWindow : MarshalByRefObject, IWin32Window
{
	private IntPtr window_handle = IntPtr.Zero;

	private static Hashtable window_collection = new Hashtable();

	[ThreadStatic]
	private static NativeWindow WindowCreating;

	public IntPtr Handle => window_handle;

	public NativeWindow()
	{
		window_handle = IntPtr.Zero;
	}

	public static NativeWindow FromHandle(IntPtr handle)
	{
		return FindFirstInTable(handle);
	}

	internal void InvalidateHandle()
	{
		RemoveFromTable(this);
		window_handle = IntPtr.Zero;
	}

	public void AssignHandle(IntPtr handle)
	{
		RemoveFromTable(this);
		window_handle = handle;
		AddToTable(this);
		OnHandleChange();
	}

	private static void AddToTable(NativeWindow window)
	{
		IntPtr handle = window.Handle;
		if (handle == IntPtr.Zero)
		{
			return;
		}
		lock (window_collection)
		{
			object obj = window_collection[handle];
			if (obj == null)
			{
				window_collection.Add(handle, window);
			}
			else if (obj is NativeWindow nativeWindow)
			{
				if (nativeWindow != window)
				{
					ArrayList arrayList = new ArrayList();
					arrayList.Add(nativeWindow);
					arrayList.Add(window);
					window_collection[handle] = arrayList;
				}
			}
			else
			{
				ArrayList arrayList2 = (ArrayList)window_collection[handle];
				if (!arrayList2.Contains(window))
				{
					arrayList2.Add(window);
				}
			}
		}
	}

	private static void RemoveFromTable(NativeWindow window)
	{
		IntPtr handle = window.Handle;
		if (handle == IntPtr.Zero)
		{
			return;
		}
		lock (window_collection)
		{
			object obj = window_collection[handle];
			if (obj == null)
			{
				return;
			}
			if (obj is NativeWindow)
			{
				window_collection.Remove(handle);
				return;
			}
			ArrayList arrayList = (ArrayList)window_collection[handle];
			arrayList.Remove(window);
			if (arrayList.Count == 0)
			{
				window_collection.Remove(handle);
			}
			else if (arrayList.Count == 1)
			{
				window_collection[handle] = arrayList[0];
			}
		}
	}

	private static NativeWindow FindFirstInTable(IntPtr handle)
	{
		if (handle == IntPtr.Zero)
		{
			return null;
		}
		NativeWindow nativeWindow = null;
		lock (window_collection)
		{
			object obj = window_collection[handle];
			if (obj != null)
			{
				nativeWindow = obj as NativeWindow;
				if (nativeWindow == null)
				{
					ArrayList arrayList = (ArrayList)obj;
					if (arrayList.Count > 0)
					{
						nativeWindow = (NativeWindow)arrayList[0];
					}
				}
			}
		}
		return nativeWindow;
	}

	public virtual void CreateHandle(CreateParams cp)
	{
		if (cp != null)
		{
			WindowCreating = this;
			window_handle = XplatUI.CreateWindow(cp);
			WindowCreating = null;
			if (window_handle != IntPtr.Zero)
			{
				AddToTable(this);
			}
		}
	}

	public void DefWndProc(ref Message m)
	{
		m.Result = XplatUI.DefWndProc(ref m);
	}

	public virtual void DestroyHandle()
	{
		if (window_handle != IntPtr.Zero)
		{
			XplatUI.DestroyWindow(window_handle);
		}
	}

	public virtual void ReleaseHandle()
	{
		RemoveFromTable(this);
		window_handle = IntPtr.Zero;
		OnHandleChange();
	}

	~NativeWindow()
	{
	}

	protected virtual void OnHandleChange()
	{
	}

	protected virtual void OnThreadException(Exception e)
	{
		Application.OnThreadException(e);
	}

	protected virtual void WndProc(ref Message m)
	{
		DefWndProc(ref m);
	}

	internal static IntPtr WndProc(IntPtr hWnd, Msg msg, IntPtr wParam, IntPtr lParam)
	{
		IntPtr result = IntPtr.Zero;
		Message m = default(Message);
		m.HWnd = hWnd;
		m.Msg = (int)msg;
		m.WParam = wParam;
		m.LParam = lParam;
		m.Result = IntPtr.Zero;
		NativeWindow nativeWindow = null;
		try
		{
			object obj = null;
			lock (window_collection)
			{
				obj = window_collection[hWnd];
			}
			nativeWindow = obj as NativeWindow;
			if (obj == null)
			{
				nativeWindow = EnsureCreated(nativeWindow, hWnd);
			}
			if (nativeWindow != null)
			{
				nativeWindow.WndProc(ref m);
				result = m.Result;
			}
			else if (obj is ArrayList)
			{
				ArrayList arrayList = (ArrayList)obj;
				lock (arrayList)
				{
					if (arrayList.Count > 0)
					{
						nativeWindow = EnsureCreated((NativeWindow)arrayList[0], hWnd);
						nativeWindow.WndProc(ref m);
						result = m.Result;
						for (int i = 1; i < arrayList.Count; i++)
						{
							((NativeWindow)arrayList[i]).WndProc(ref m);
						}
					}
				}
			}
			else
			{
				result = XplatUI.DefWndProc(ref m);
			}
		}
		catch (Exception e)
		{
			nativeWindow?.OnThreadException(e);
		}
		return result;
	}

	private static NativeWindow EnsureCreated(NativeWindow window, IntPtr hWnd)
	{
		if (window == null && WindowCreating != null)
		{
			window = WindowCreating;
			WindowCreating = null;
			if (window.Handle == IntPtr.Zero)
			{
				window.AssignHandle(hWnd);
			}
		}
		return window;
	}
}
