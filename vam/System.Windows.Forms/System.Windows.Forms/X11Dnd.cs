using System.Collections;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;

namespace System.Windows.Forms;

internal class X11Dnd
{
	private enum State
	{
		Accepting,
		Dragging
	}

	private enum DragState
	{
		None,
		Beginning,
		Dragging,
		Entered
	}

	private interface IDataConverter
	{
		void GetData(X11Dnd dnd, IDataObject data, ref XEvent xevent);

		void SetData(X11Dnd dnd, object data, ref XEvent xevent);
	}

	private class MimeHandler
	{
		public string Name;

		public string[] Aliases;

		public IntPtr Type;

		public IntPtr NonProtocol;

		public IDataConverter Converter;

		public MimeHandler(string name, IDataConverter converter)
			: this(name, converter, name)
		{
		}

		public MimeHandler(string name, IDataConverter converter, params string[] aliases)
		{
			Name = name;
			Converter = converter;
			Aliases = aliases;
		}

		public override string ToString()
		{
			return "MimeHandler {" + Name + "}";
		}
	}

	private class SerializedObjectConverter : IDataConverter
	{
		public void GetData(X11Dnd dnd, IDataObject data, ref XEvent xevent)
		{
			MemoryStream data2 = dnd.GetData(ref xevent);
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			if (data2.Length != 0L)
			{
				data2.Seek(0L, SeekOrigin.Begin);
				object data3 = binaryFormatter.Deserialize(data2);
				data.SetData(data3);
			}
		}

		public void SetData(X11Dnd dnd, object data, ref XEvent xevent)
		{
			if (data != null)
			{
				MemoryStream memoryStream = new MemoryStream();
				BinaryFormatter binaryFormatter = new BinaryFormatter();
				binaryFormatter.Serialize(memoryStream, data);
				IntPtr intPtr = Marshal.AllocHGlobal((int)memoryStream.Length);
				memoryStream.Seek(0L, SeekOrigin.Begin);
				for (int i = 0; i < memoryStream.Length; i++)
				{
					Marshal.WriteByte(intPtr, i, (byte)memoryStream.ReadByte());
				}
				dnd.SetProperty(ref xevent, intPtr, (int)memoryStream.Length);
			}
		}
	}

	private class HtmlConverter : IDataConverter
	{
		public void GetData(X11Dnd dnd, IDataObject data, ref XEvent xevent)
		{
			string text = dnd.GetText(ref xevent, unicode: false);
			if (text != null)
			{
				data.SetData(DataFormats.Text, text);
				data.SetData(DataFormats.UnicodeText, text);
			}
		}

		public void SetData(X11Dnd dnd, object data, ref XEvent xevent)
		{
			if (!(data is string s))
			{
				return;
			}
			IntPtr intPtr;
			int i;
			if (xevent.SelectionRequestEvent.target == (IntPtr)31)
			{
				byte[] bytes = Encoding.ASCII.GetBytes(s);
				intPtr = Marshal.AllocHGlobal(bytes.Length);
				i = bytes.Length;
				for (int j = 0; j < i; j++)
				{
					Marshal.WriteByte(intPtr, j, bytes[j]);
				}
			}
			else
			{
				intPtr = Marshal.StringToHGlobalAnsi(s);
				for (i = 0; Marshal.ReadByte(intPtr, i) != 0; i++)
				{
				}
			}
			dnd.SetProperty(ref xevent, intPtr, i);
			Marshal.FreeHGlobal(intPtr);
		}
	}

	private class TextConverter : IDataConverter
	{
		public void GetData(X11Dnd dnd, IDataObject data, ref XEvent xevent)
		{
			string text = dnd.GetText(ref xevent, unicode: true);
			if (text != null)
			{
				data.SetData(DataFormats.Text, text);
				data.SetData(DataFormats.UnicodeText, text);
			}
		}

		public void SetData(X11Dnd dnd, object data, ref XEvent xevent)
		{
			string text = data as string;
			if (text == null)
			{
				if (!(data is IDataObject dataObject))
				{
					return;
				}
				text = (string)dataObject.GetData("System.String", autoConvert: true);
			}
			IntPtr intPtr;
			int i;
			if (xevent.SelectionRequestEvent.target == (IntPtr)31)
			{
				byte[] bytes = Encoding.ASCII.GetBytes(text);
				intPtr = Marshal.AllocHGlobal(bytes.Length);
				i = bytes.Length;
				for (int j = 0; j < i; j++)
				{
					Marshal.WriteByte(intPtr, j, bytes[j]);
				}
			}
			else
			{
				intPtr = Marshal.StringToHGlobalAnsi(text);
				for (i = 0; Marshal.ReadByte(intPtr, i) != 0; i++)
				{
				}
			}
			dnd.SetProperty(ref xevent, intPtr, i);
			Marshal.FreeHGlobal(intPtr);
		}
	}

	private class UriListConverter : IDataConverter
	{
		public void GetData(X11Dnd dnd, IDataObject data, ref XEvent xevent)
		{
			string text = dnd.GetText(ref xevent, unicode: false);
			if (text == null)
			{
				return;
			}
			ArrayList arrayList = new ArrayList();
			string[] array = text.Split('\r', '\n');
			string[] array2 = array;
			foreach (string text2 in array2)
			{
				if (!text2.StartsWith("#"))
				{
					try
					{
						Uri uri = new Uri(text2);
						arrayList.Add(uri.LocalPath);
					}
					catch
					{
					}
				}
			}
			string[] array3 = (string[])arrayList.ToArray(typeof(string));
			if (array3.Length >= 1)
			{
				data.SetData(DataFormats.FileDrop, array3);
				data.SetData("FileName", array3[0]);
				data.SetData("FileNameW", array3[0]);
			}
		}

		public void SetData(X11Dnd dnd, object data, ref XEvent xevent)
		{
			string[] array = data as string[];
			if (array == null)
			{
				if (!(data is IDataObject dataObject))
				{
					return;
				}
				array = dataObject.GetData(DataFormats.FileDrop, autoConvert: true) as string[];
			}
			if (array != null)
			{
				StringBuilder stringBuilder = new StringBuilder();
				string[] array2 = array;
				foreach (string uriString in array2)
				{
					Uri uri = new Uri(uriString);
					stringBuilder.Append(uri.ToString());
					stringBuilder.Append("\r\n");
				}
				IntPtr intPtr = Marshal.StringToHGlobalAnsi(stringBuilder.ToString());
				int j;
				for (j = 0; Marshal.ReadByte(intPtr, j) != 0; j++)
				{
				}
				dnd.SetProperty(ref xevent, intPtr, j);
			}
		}
	}

	private class DragData
	{
		public IntPtr Window;

		public DragState State;

		public object Data;

		public IntPtr Action;

		public IntPtr[] SupportedTypes;

		public MouseButtons MouseState;

		public DragDropEffects AllowedEffects;

		public Point CurMousePos;

		public IntPtr LastWindow;

		public IntPtr LastTopLevel;

		public bool WillAccept;

		public void Reset()
		{
			State = DragState.None;
			Data = null;
			SupportedTypes = null;
			WillAccept = false;
		}
	}

	private delegate void MimeConverter(IntPtr dsp, IDataObject data, ref XEvent xevent);

	private MimeHandler[] MimeHandlers = new MimeHandler[5]
	{
		new MimeHandler("text/plain", new TextConverter()),
		new MimeHandler("text/plain", new TextConverter(), "System.String", DataFormats.Text),
		new MimeHandler("text/html", new HtmlConverter(), DataFormats.Html),
		new MimeHandler("text/uri-list", new UriListConverter(), DataFormats.FileDrop),
		new MimeHandler("application/x-mono-serialized-object", new SerializedObjectConverter())
	};

	private static readonly IntPtr[] XdndVersion = new IntPtr[1]
	{
		new IntPtr(4)
	};

	private IntPtr display;

	private DragData drag_data;

	private IntPtr XdndAware;

	private IntPtr XdndSelection;

	private IntPtr XdndEnter;

	private IntPtr XdndLeave;

	private IntPtr XdndPosition;

	private IntPtr XdndDrop;

	private IntPtr XdndFinished;

	private IntPtr XdndStatus;

	private IntPtr XdndTypeList;

	private IntPtr XdndActionCopy;

	private IntPtr XdndActionMove;

	private IntPtr XdndActionLink;

	private IntPtr XdndActionList;

	private int converts_pending;

	private bool position_recieved;

	private bool status_sent;

	private IntPtr target;

	private IntPtr source;

	private IntPtr toplevel;

	private IDataObject data;

	private Control control;

	private int pos_x;

	private int pos_y;

	private DragDropEffects allowed;

	private DragEventArgs drag_event;

	private Cursor CursorNo;

	private Cursor CursorCopy;

	private Cursor CursorMove;

	private Cursor CursorLink;

	private bool tracking;

	private bool dropped;

	private int motion_poll;

	public X11Dnd(IntPtr display, X11Keyboard keyboard)
	{
		this.display = display;
		Init();
	}

	public bool InDrag()
	{
		if (drag_data == null)
		{
			return false;
		}
		return drag_data.State != DragState.None;
	}

	public void SetAllowDrop(Hwnd hwnd, bool allow)
	{
		if (hwnd.allow_drop != allow)
		{
			int[] array = new int[XdndVersion.Length];
			for (int i = 0; i < XdndVersion.Length; i++)
			{
				array[i] = XdndVersion[i].ToInt32();
			}
			XplatUIX11.XChangeProperty(display, hwnd.whole_window, XdndAware, (IntPtr)4, 32, PropertyMode.Replace, array, allow ? 1 : 0);
			hwnd.allow_drop = allow;
		}
	}

	public DragDropEffects StartDrag(IntPtr handle, object data, DragDropEffects allowed_effects)
	{
		drag_data = new DragData();
		drag_data.Window = handle;
		drag_data.State = DragState.Beginning;
		drag_data.MouseState = XplatUIX11.MouseState;
		drag_data.Data = data;
		drag_data.SupportedTypes = DetermineSupportedTypes(data);
		drag_data.AllowedEffects = allowed_effects;
		drag_data.Action = ActionFromEffect(allowed_effects);
		if (CursorNo == null)
		{
			CursorNo = new Cursor(typeof(X11Dnd), "DnDNo.cur");
			CursorCopy = new Cursor(typeof(X11Dnd), "DnDCopy.cur");
			CursorMove = new Cursor(typeof(X11Dnd), "DnDMove.cur");
			CursorLink = new Cursor(typeof(X11Dnd), "DnDLink.cur");
		}
		drag_data.LastTopLevel = IntPtr.Zero;
		control = null;
		MSG msg = default(MSG);
		object queue_id = XplatUI.StartLoop(Thread.CurrentThread);
		Timer timer = new Timer();
		timer.Tick += DndTickHandler;
		timer.Interval = 100;
		drag_data.State = DragState.Dragging;
		if (XplatUIX11.XSetSelectionOwner(display, XdndSelection, drag_data.Window, IntPtr.Zero) == 0)
		{
			Console.Error.WriteLine("Could not take ownership of XdndSelection aborting drag.");
			drag_data.Reset();
			return DragDropEffects.None;
		}
		drag_data.State = DragState.Dragging;
		drag_data.CurMousePos = default(Point);
		source = (toplevel = (target = IntPtr.Zero));
		dropped = false;
		tracking = true;
		motion_poll = -1;
		timer.Start();
		SendEnter(drag_data.Window, drag_data.Window, drag_data.SupportedTypes);
		drag_data.LastTopLevel = toplevel;
		while (tracking && XplatUI.GetMessage(queue_id, ref msg, IntPtr.Zero, 0, 0))
		{
			if (msg.message >= Msg.WM_KEYDOWN && msg.message <= Msg.WM_KEYLAST)
			{
				HandleKeyMessage(msg);
				continue;
			}
			switch (msg.message)
			{
			case Msg.WM_LBUTTONUP:
			case Msg.WM_RBUTTONUP:
			case Msg.WM_MBUTTONUP:
				if ((msg.message == Msg.WM_LBUTTONDOWN && drag_data.MouseState != MouseButtons.Left) || (msg.message == Msg.WM_RBUTTONDOWN && drag_data.MouseState != MouseButtons.Right) || (msg.message == Msg.WM_MBUTTONDOWN && drag_data.MouseState != MouseButtons.Middle))
				{
					break;
				}
				HandleButtonUpMsg();
				RemoveCapture(msg.hwnd);
				continue;
			case Msg.WM_MOUSEMOVE:
				motion_poll = 0;
				drag_data.CurMousePos.X = Control.LowOrder(msg.lParam.ToInt32());
				drag_data.CurMousePos.Y = Control.HighOrder(msg.lParam.ToInt32());
				HandleMouseOver();
				continue;
			}
			XplatUI.DispatchMessage(ref msg);
		}
		timer.Stop();
		if (control != null)
		{
			Application.DoEvents();
		}
		if (!dropped)
		{
			return DragDropEffects.None;
		}
		if (drag_event != null)
		{
			return drag_event.Effect;
		}
		return DragDropEffects.None;
	}

	private void DndTickHandler(object sender, EventArgs e)
	{
		if (dropped)
		{
			Timer timer = (Timer)sender;
			if (timer.Interval == 500)
			{
				tracking = false;
			}
			else
			{
				timer.Interval = 500;
			}
		}
		if (motion_poll > 1)
		{
			HandleMouseOver();
		}
		else if (motion_poll > -1)
		{
			motion_poll++;
		}
	}

	private void DefaultEnterLeave(object user_data)
	{
		GetWindowsUnderPointer(out var window, out var _, out var _, out var _);
		Control control = Control.FromHandle(window);
		if (control != null && control.AllowDrop)
		{
			Point mousePosition = Control.MousePosition;
			DragEventArgs dragEventArgs = new DragEventArgs(data, 0, mousePosition.X, mousePosition.Y, drag_data.AllowedEffects, DragDropEffects.None);
			control.DndEnter(dragEventArgs);
			if ((dragEventArgs.Effect & drag_data.AllowedEffects) != 0)
			{
				control.DndDrop(dragEventArgs);
			}
			else
			{
				control.DndLeave(EventArgs.Empty);
			}
		}
	}

	public void HandleButtonUpMsg()
	{
		if (drag_data.State == DragState.Beginning || drag_data.State == DragState.None)
		{
			return;
		}
		if (drag_data.WillAccept)
		{
			if (QueryContinue(escape: false, DragAction.Drop))
			{
				return;
			}
		}
		else
		{
			if (QueryContinue(escape: false, DragAction.Cancel))
			{
				return;
			}
			if (motion_poll == -1)
			{
				DefaultEnterLeave(drag_data.Data);
			}
		}
		drag_data.State = DragState.None;
	}

	private void RemoveCapture(IntPtr handle)
	{
		Control control = MwfWindow(handle);
		if (control.InternalCapture)
		{
			control.InternalCapture = false;
		}
	}

	public bool HandleMouseOver()
	{
		GetWindowsUnderPointer(out var window, out var intPtr, out var x_root, out var y_root);
		if (window != drag_data.LastWindow && drag_data.State == DragState.Entered)
		{
			drag_data.State = DragState.Dragging;
			if (intPtr != drag_data.LastTopLevel)
			{
				SendLeave(drag_data.LastTopLevel, intPtr);
			}
		}
		drag_data.State = DragState.Entered;
		if (intPtr != drag_data.LastTopLevel)
		{
			SendEnter(intPtr, drag_data.Window, drag_data.SupportedTypes);
		}
		else
		{
			SendPosition(intPtr, drag_data.Window, drag_data.Action, x_root, y_root, IntPtr.Zero);
		}
		drag_data.LastTopLevel = intPtr;
		drag_data.LastWindow = window;
		return true;
	}

	private void GetWindowsUnderPointer(out IntPtr window, out IntPtr toplevel, out int x_root, out int y_root)
	{
		toplevel = IntPtr.Zero;
		window = XplatUIX11.RootWindowHandle;
		bool flag = false;
		int win_x = (x_root = drag_data.CurMousePos.X);
		int win_y = (y_root = drag_data.CurMousePos.Y);
		IntPtr root;
		IntPtr child;
		int root_x;
		int root_y;
		int keys_buttons;
		while (XplatUIX11.XQueryPointer(display, window, out root, out child, out root_x, out root_y, out win_x, out win_y, out keys_buttons))
		{
			if (!flag)
			{
				flag = IsWindowDndAware(window);
				if (flag)
				{
					toplevel = window;
					x_root = root_x;
					y_root = root_y;
				}
			}
			if (child == IntPtr.Zero)
			{
				break;
			}
			window = child;
		}
	}

	public void HandleKeyMessage(MSG msg)
	{
		if (msg.wParam.ToInt32() == 27)
		{
			QueryContinue(escape: true, DragAction.Cancel);
		}
	}

	public bool HandleClientMessage(ref XEvent xevent)
	{
		if (xevent.ClientMessageEvent.message_type == XdndPosition)
		{
			return Accepting_HandlePositionEvent(ref xevent);
		}
		if (xevent.ClientMessageEvent.message_type == XdndEnter)
		{
			return Accepting_HandleEnterEvent(ref xevent);
		}
		if (xevent.ClientMessageEvent.message_type == XdndDrop)
		{
			return Accepting_HandleDropEvent(ref xevent);
		}
		if (xevent.ClientMessageEvent.message_type == XdndLeave)
		{
			return Accepting_HandleLeaveEvent(ref xevent);
		}
		if (xevent.ClientMessageEvent.message_type == XdndStatus)
		{
			return HandleStatusEvent(ref xevent);
		}
		if (xevent.ClientMessageEvent.message_type == XdndFinished)
		{
			return HandleFinishedEvent(ref xevent);
		}
		return false;
	}

	public bool HandleSelectionNotifyEvent(ref XEvent xevent)
	{
		MimeHandler mimeHandler = FindHandler(xevent.SelectionEvent.target);
		if (mimeHandler == null)
		{
			return false;
		}
		if (data == null)
		{
			data = new DataObject();
		}
		mimeHandler.Converter.GetData(this, data, ref xevent);
		converts_pending--;
		if (converts_pending <= 0 && position_recieved)
		{
			drag_event = new DragEventArgs(data, 0, pos_x, pos_y, allowed, DragDropEffects.None);
			control.DndEnter(drag_event);
			SendStatus(source, drag_event.Effect);
			status_sent = true;
		}
		return true;
	}

	public bool HandleSelectionRequestEvent(ref XEvent xevent)
	{
		if (xevent.SelectionRequestEvent.selection != XdndSelection)
		{
			return false;
		}
		MimeHandler mimeHandler = FindHandler(xevent.SelectionRequestEvent.target);
		if (mimeHandler == null)
		{
			return false;
		}
		mimeHandler.Converter.SetData(this, drag_data.Data, ref xevent);
		return true;
	}

	private bool QueryContinue(bool escape, DragAction action)
	{
		QueryContinueDragEventArgs queryContinueDragEventArgs = new QueryContinueDragEventArgs((int)XplatUI.State.ModifierKeys, escape, action);
		Control control = MwfWindow(source);
		if (control == null)
		{
			tracking = false;
			return false;
		}
		control.DndContinueDrag(queryContinueDragEventArgs);
		switch (queryContinueDragEventArgs.Action)
		{
		case DragAction.Continue:
			return true;
		case DragAction.Drop:
			SendDrop(drag_data.LastTopLevel, source, IntPtr.Zero);
			tracking = false;
			return true;
		case DragAction.Cancel:
			drag_data.Reset();
			control.InternalCapture = false;
			break;
		}
		SendLeave(drag_data.LastTopLevel, toplevel);
		RestoreDefaultCursor();
		tracking = false;
		return false;
	}

	private void RestoreDefaultCursor()
	{
		XplatUIX11.XChangeActivePointerGrab(display, EventMask.ButtonPressMask | EventMask.ButtonReleaseMask | EventMask.PointerMotionMask | EventMask.ButtonMotionMask, Cursors.Default.Handle, IntPtr.Zero);
	}

	private void GiveFeedback(IntPtr action)
	{
		GiveFeedbackEventArgs giveFeedbackEventArgs = new GiveFeedbackEventArgs(EffectFromAction(drag_data.Action), useDefaultCursors: true);
		Control control = MwfWindow(source);
		control.DndFeedback(giveFeedbackEventArgs);
		if (!giveFeedbackEventArgs.UseDefaultCursors)
		{
			return;
		}
		Cursor cursor = CursorNo;
		if (drag_data.WillAccept)
		{
			if (action == XdndActionCopy)
			{
				cursor = CursorCopy;
			}
			else if (action == XdndActionLink)
			{
				cursor = CursorLink;
			}
			else if (action == XdndActionMove)
			{
				cursor = CursorMove;
			}
		}
		XplatUIX11.XChangeActivePointerGrab(display, EventMask.ButtonPressMask | EventMask.ButtonReleaseMask | EventMask.PointerMotionMask | EventMask.ButtonMotionMask, cursor.Handle, IntPtr.Zero);
	}

	private void SetProperty(ref XEvent xevent, IntPtr data, int length)
	{
		XEvent send_event = default(XEvent);
		send_event.SelectionEvent.type = XEventName.SelectionNotify;
		send_event.SelectionEvent.send_event = true;
		send_event.SelectionEvent.display = display;
		send_event.SelectionEvent.selection = xevent.SelectionRequestEvent.selection;
		send_event.SelectionEvent.target = xevent.SelectionRequestEvent.target;
		send_event.SelectionEvent.requestor = xevent.SelectionRequestEvent.requestor;
		send_event.SelectionEvent.time = xevent.SelectionRequestEvent.time;
		send_event.SelectionEvent.property = IntPtr.Zero;
		XplatUIX11.XChangeProperty(display, xevent.SelectionRequestEvent.requestor, xevent.SelectionRequestEvent.property, xevent.SelectionRequestEvent.target, 8, PropertyMode.Replace, data, length);
		send_event.SelectionEvent.property = xevent.SelectionRequestEvent.property;
		XplatUIX11.XSendEvent(display, xevent.SelectionRequestEvent.requestor, propagate: false, (IntPtr)0, ref send_event);
	}

	private void Reset()
	{
		ResetSourceData();
		ResetTargetData();
	}

	private void ResetSourceData()
	{
		converts_pending = 0;
		data = null;
	}

	private void ResetTargetData()
	{
		position_recieved = false;
		status_sent = false;
	}

	private bool Accepting_HandleEnterEvent(ref XEvent xevent)
	{
		Reset();
		source = xevent.ClientMessageEvent.ptr1;
		toplevel = xevent.AnyEvent.window;
		target = IntPtr.Zero;
		ConvertData(ref xevent);
		return true;
	}

	private bool Accepting_HandlePositionEvent(ref XEvent xevent)
	{
		pos_x = (int)xevent.ClientMessageEvent.ptr3 >> 16;
		pos_y = (int)xevent.ClientMessageEvent.ptr3 & 0xFFFF;
		Control control = MwfWindow(source);
		if (control == null)
		{
			allowed = EffectsFromX11Source(source, xevent.ClientMessageEvent.ptr5) | DragDropEffects.Copy;
		}
		else
		{
			allowed = drag_data.AllowedEffects;
		}
		IntPtr src_w = XplatUIX11.XRootWindow(display, 0);
		IntPtr intPtr = toplevel;
		IntPtr intPtr2 = IntPtr.Zero;
		while (true)
		{
			IntPtr child_return = IntPtr.Zero;
			if (!XplatUIX11.XTranslateCoordinates(display, src_w, intPtr, pos_x, pos_y, out var _, out var _, out child_return) || child_return == IntPtr.Zero)
			{
				break;
			}
			intPtr = child_return;
			Hwnd hwnd = Hwnd.ObjectFromHandle(intPtr);
			Control control2 = Control.FromHandle(hwnd.client_window);
			if (control2 != null && control2.allow_drop)
			{
				intPtr2 = intPtr;
			}
		}
		if (intPtr2 != IntPtr.Zero)
		{
			intPtr = intPtr2;
		}
		if (target != intPtr)
		{
			Finish();
		}
		target = intPtr;
		Hwnd hwnd2 = Hwnd.ObjectFromHandle(target);
		Control control3 = Control.FromHandle(hwnd2.client_window);
		if (control3 == null)
		{
			return true;
		}
		if (!control3.allow_drop)
		{
			SendStatus(source, DragDropEffects.None);
			Finish();
			return true;
		}
		this.control = control3;
		position_recieved = true;
		if (converts_pending > 0)
		{
			return true;
		}
		if (!status_sent)
		{
			drag_event = new DragEventArgs(data, 0, pos_x, pos_y, allowed, DragDropEffects.None);
			this.control.DndEnter(drag_event);
			SendStatus(source, drag_event.Effect);
			status_sent = true;
		}
		else
		{
			drag_event.x = pos_x;
			drag_event.y = pos_y;
			this.control.DndOver(drag_event);
			SendStatus(source, drag_event.Effect);
		}
		return true;
	}

	private void Finish()
	{
		if (control != null)
		{
			if (drag_event == null)
			{
				if (data == null)
				{
					data = new DataObject();
				}
				drag_event = new DragEventArgs(data, 0, pos_x, pos_y, allowed, DragDropEffects.None);
			}
			control.DndLeave(drag_event);
			control = null;
		}
		ResetTargetData();
	}

	private bool Accepting_HandleDropEvent(ref XEvent xevent)
	{
		if (control != null && drag_event != null)
		{
			drag_event = new DragEventArgs(data, 0, pos_x, pos_y, allowed, drag_event.Effect);
			control.DndDrop(drag_event);
		}
		SendFinished();
		return true;
	}

	private bool Accepting_HandleLeaveEvent(ref XEvent xevent)
	{
		if (control != null && drag_event != null)
		{
			control.DndLeave(drag_event);
		}
		return true;
	}

	private bool HandleStatusEvent(ref XEvent xevent)
	{
		if (drag_data != null && drag_data.State == DragState.Entered)
		{
			if (!QueryContinue(escape: false, DragAction.Continue))
			{
				return true;
			}
			drag_data.WillAccept = ((int)xevent.ClientMessageEvent.ptr2 & 1) != 0;
			GiveFeedback(xevent.ClientMessageEvent.ptr5);
		}
		return true;
	}

	private bool HandleFinishedEvent(ref XEvent xevent)
	{
		return true;
	}

	private DragDropEffects EffectsFromX11Source(IntPtr source, IntPtr action_atom)
	{
		DragDropEffects dragDropEffects = DragDropEffects.None;
		IntPtr prop = IntPtr.Zero;
		XplatUIX11.XGetWindowProperty(display, source, XdndActionList, IntPtr.Zero, new IntPtr(32), delete: false, (IntPtr)0, out var _, out var _, out var nitems, out var _, ref prop);
		int num = Marshal.SizeOf(typeof(IntPtr));
		for (int i = 0; i < nitems.ToInt32(); i++)
		{
			IntPtr action = Marshal.ReadIntPtr(prop, i * num);
			dragDropEffects |= EffectFromAction(action);
		}
		if (dragDropEffects == DragDropEffects.None)
		{
			dragDropEffects = EffectFromAction(action_atom);
		}
		return dragDropEffects;
	}

	private DragDropEffects EffectFromAction(IntPtr action)
	{
		if (action == XdndActionCopy)
		{
			return DragDropEffects.Copy;
		}
		if (action == XdndActionMove)
		{
			return DragDropEffects.Move;
		}
		if (action == XdndActionLink)
		{
			return DragDropEffects.Link;
		}
		return DragDropEffects.None;
	}

	private IntPtr ActionFromEffect(DragDropEffects effect)
	{
		IntPtr result = IntPtr.Zero;
		if ((effect & DragDropEffects.Copy) != 0)
		{
			result = XdndActionCopy;
		}
		else if ((effect & DragDropEffects.Move) != 0)
		{
			result = XdndActionMove;
		}
		else if ((effect & DragDropEffects.Link) != 0)
		{
			result = XdndActionLink;
		}
		return result;
	}

	private bool ConvertData(ref XEvent xevent)
	{
		bool result = false;
		Control control = MwfWindow(source);
		if (control != null && drag_data != null)
		{
			if (!tracking)
			{
				return false;
			}
			if (drag_data.Data is IDataObject dataObject)
			{
				data = dataObject;
			}
			else
			{
				if (data == null)
				{
					data = new DataObject();
				}
				SetDataWithFormats(drag_data.Data);
			}
			return true;
		}
		IntPtr[] array = SourceSupportedList(ref xevent);
		foreach (IntPtr atom in array)
		{
			MimeHandler mimeHandler = FindHandler(atom);
			if (mimeHandler != null)
			{
				XplatUIX11.XConvertSelection(display, XdndSelection, mimeHandler.Type, mimeHandler.NonProtocol, toplevel, IntPtr.Zero);
				converts_pending++;
				result = true;
			}
		}
		return result;
	}

	private void SetDataWithFormats(object value)
	{
		if (value is string)
		{
			data.SetData(DataFormats.Text, value);
			data.SetData(DataFormats.UnicodeText, value);
		}
		data.SetData(value);
	}

	private MimeHandler FindHandler(IntPtr atom)
	{
		if (atom == IntPtr.Zero)
		{
			return null;
		}
		MimeHandler[] mimeHandlers = MimeHandlers;
		foreach (MimeHandler mimeHandler in mimeHandlers)
		{
			if (mimeHandler.Type == atom)
			{
				return mimeHandler;
			}
		}
		return null;
	}

	private MimeHandler FindHandler(string name)
	{
		MimeHandler[] mimeHandlers = MimeHandlers;
		foreach (MimeHandler mimeHandler in mimeHandlers)
		{
			string[] aliases = mimeHandler.Aliases;
			foreach (string text in aliases)
			{
				if (text == name)
				{
					return mimeHandler;
				}
			}
		}
		return null;
	}

	private void SendStatus(IntPtr source, DragDropEffects effect)
	{
		XEvent send_event = default(XEvent);
		send_event.AnyEvent.type = XEventName.ClientMessage;
		send_event.AnyEvent.display = display;
		send_event.ClientMessageEvent.window = source;
		send_event.ClientMessageEvent.message_type = XdndStatus;
		send_event.ClientMessageEvent.format = 32;
		send_event.ClientMessageEvent.ptr1 = toplevel;
		if (effect != 0 && (effect & allowed) != 0)
		{
			send_event.ClientMessageEvent.ptr2 = (IntPtr)1;
		}
		send_event.ClientMessageEvent.ptr5 = ActionFromEffect(effect);
		XplatUIX11.XSendEvent(display, source, propagate: false, IntPtr.Zero, ref send_event);
	}

	private void SendEnter(IntPtr handle, IntPtr from, IntPtr[] supported)
	{
		XEvent send_event = default(XEvent);
		send_event.AnyEvent.type = XEventName.ClientMessage;
		send_event.AnyEvent.display = display;
		send_event.ClientMessageEvent.window = handle;
		send_event.ClientMessageEvent.message_type = XdndEnter;
		send_event.ClientMessageEvent.format = 32;
		send_event.ClientMessageEvent.ptr1 = from;
		send_event.ClientMessageEvent.ptr2 = (IntPtr)((long)XdndVersion[0] << 24);
		if (supported.Length > 0)
		{
			send_event.ClientMessageEvent.ptr3 = supported[0];
		}
		if (supported.Length > 1)
		{
			send_event.ClientMessageEvent.ptr4 = supported[1];
		}
		if (supported.Length > 2)
		{
			send_event.ClientMessageEvent.ptr5 = supported[2];
		}
		XplatUIX11.XSendEvent(display, handle, propagate: false, IntPtr.Zero, ref send_event);
	}

	private void SendDrop(IntPtr handle, IntPtr from, IntPtr time)
	{
		XEvent send_event = default(XEvent);
		send_event.AnyEvent.type = XEventName.ClientMessage;
		send_event.AnyEvent.display = display;
		send_event.ClientMessageEvent.window = handle;
		send_event.ClientMessageEvent.message_type = XdndDrop;
		send_event.ClientMessageEvent.format = 32;
		send_event.ClientMessageEvent.ptr1 = from;
		send_event.ClientMessageEvent.ptr3 = time;
		XplatUIX11.XSendEvent(display, handle, propagate: false, IntPtr.Zero, ref send_event);
		dropped = true;
	}

	private void SendPosition(IntPtr handle, IntPtr from, IntPtr action, int x, int y, IntPtr time)
	{
		XEvent send_event = default(XEvent);
		send_event.AnyEvent.type = XEventName.ClientMessage;
		send_event.AnyEvent.display = display;
		send_event.ClientMessageEvent.window = handle;
		send_event.ClientMessageEvent.message_type = XdndPosition;
		send_event.ClientMessageEvent.format = 32;
		send_event.ClientMessageEvent.ptr1 = from;
		send_event.ClientMessageEvent.ptr3 = (IntPtr)((x << 16) | (y & 0xFFFF));
		send_event.ClientMessageEvent.ptr4 = time;
		send_event.ClientMessageEvent.ptr5 = action;
		XplatUIX11.XSendEvent(display, handle, propagate: false, IntPtr.Zero, ref send_event);
	}

	private void SendLeave(IntPtr handle, IntPtr from)
	{
		XEvent send_event = default(XEvent);
		send_event.AnyEvent.type = XEventName.ClientMessage;
		send_event.AnyEvent.display = display;
		send_event.ClientMessageEvent.window = handle;
		send_event.ClientMessageEvent.message_type = XdndLeave;
		send_event.ClientMessageEvent.format = 32;
		send_event.ClientMessageEvent.ptr1 = from;
		XplatUIX11.XSendEvent(display, handle, propagate: false, IntPtr.Zero, ref send_event);
	}

	private void SendFinished()
	{
		XEvent send_event = default(XEvent);
		send_event.AnyEvent.type = XEventName.ClientMessage;
		send_event.AnyEvent.display = display;
		send_event.ClientMessageEvent.window = source;
		send_event.ClientMessageEvent.message_type = XdndFinished;
		send_event.ClientMessageEvent.format = 32;
		send_event.ClientMessageEvent.ptr1 = toplevel;
		XplatUIX11.XSendEvent(display, source, propagate: false, IntPtr.Zero, ref send_event);
	}

	private void Init()
	{
		XdndAware = XplatUIX11.XInternAtom(display, "XdndAware", only_if_exists: false);
		XdndEnter = XplatUIX11.XInternAtom(display, "XdndEnter", only_if_exists: false);
		XdndLeave = XplatUIX11.XInternAtom(display, "XdndLeave", only_if_exists: false);
		XdndPosition = XplatUIX11.XInternAtom(display, "XdndPosition", only_if_exists: false);
		XdndStatus = XplatUIX11.XInternAtom(display, "XdndStatus", only_if_exists: false);
		XdndDrop = XplatUIX11.XInternAtom(display, "XdndDrop", only_if_exists: false);
		XdndSelection = XplatUIX11.XInternAtom(display, "XdndSelection", only_if_exists: false);
		XdndFinished = XplatUIX11.XInternAtom(display, "XdndFinished", only_if_exists: false);
		XdndTypeList = XplatUIX11.XInternAtom(display, "XdndTypeList", only_if_exists: false);
		XdndActionCopy = XplatUIX11.XInternAtom(display, "XdndActionCopy", only_if_exists: false);
		XdndActionMove = XplatUIX11.XInternAtom(display, "XdndActionMove", only_if_exists: false);
		XdndActionLink = XplatUIX11.XInternAtom(display, "XdndActionLink", only_if_exists: false);
		XdndActionList = XplatUIX11.XInternAtom(display, "XdndActionList", only_if_exists: false);
		MimeHandler[] mimeHandlers = MimeHandlers;
		foreach (MimeHandler mimeHandler in mimeHandlers)
		{
			mimeHandler.Type = XplatUIX11.XInternAtom(display, mimeHandler.Name, only_if_exists: false);
			mimeHandler.NonProtocol = XplatUIX11.XInternAtom(display, "MWFNonP+" + mimeHandler.Name, only_if_exists: false);
		}
	}

	private IntPtr[] SourceSupportedList(ref XEvent xevent)
	{
		IntPtr[] array;
		if (((int)xevent.ClientMessageEvent.ptr2 & 1) == 0)
		{
			array = new IntPtr[3]
			{
				xevent.ClientMessageEvent.ptr3,
				xevent.ClientMessageEvent.ptr4,
				xevent.ClientMessageEvent.ptr5
			};
		}
		else
		{
			IntPtr prop = IntPtr.Zero;
			XplatUIX11.XGetWindowProperty(display, source, XdndTypeList, IntPtr.Zero, new IntPtr(32), delete: false, (IntPtr)4, out var _, out var _, out var nitems, out var _, ref prop);
			array = new IntPtr[nitems.ToInt32()];
			for (int i = 0; i < nitems.ToInt32(); i++)
			{
				ref IntPtr reference = ref array[i];
				reference = (IntPtr)Marshal.ReadInt32(prop, i * Marshal.SizeOf(typeof(int)));
			}
			XplatUIX11.XFree(prop);
		}
		return array;
	}

	private string GetText(ref XEvent xevent, bool unicode)
	{
		int num = 0;
		StringBuilder stringBuilder = new StringBuilder();
		IntPtr bytes_after;
		do
		{
			IntPtr prop = IntPtr.Zero;
			if (XplatUIX11.XGetWindowProperty(display, xevent.AnyEvent.window, xevent.SelectionEvent.property, IntPtr.Zero, new IntPtr(16777215), delete: false, (IntPtr)0, out var _, out var _, out var nitems, out bytes_after, ref prop) != 0)
			{
				XplatUIX11.XFree(prop);
				break;
			}
			if (unicode)
			{
				stringBuilder.Append(Marshal.PtrToStringUni(prop));
			}
			else
			{
				stringBuilder.Append(Marshal.PtrToStringAnsi(prop));
			}
			num += nitems.ToInt32();
			XplatUIX11.XFree(prop);
		}
		while (bytes_after.ToInt32() > 0);
		if (num == 0)
		{
			return null;
		}
		return stringBuilder.ToString();
	}

	private MemoryStream GetData(ref XEvent xevent)
	{
		int num = 0;
		MemoryStream memoryStream = new MemoryStream();
		IntPtr bytes_after;
		do
		{
			IntPtr prop = IntPtr.Zero;
			if (XplatUIX11.XGetWindowProperty(display, xevent.AnyEvent.window, xevent.SelectionEvent.property, IntPtr.Zero, new IntPtr(16777215), delete: false, (IntPtr)0, out var _, out var _, out var nitems, out bytes_after, ref prop) != 0)
			{
				XplatUIX11.XFree(prop);
				break;
			}
			for (int i = 0; i < nitems.ToInt32(); i++)
			{
				memoryStream.WriteByte(Marshal.ReadByte(prop, i));
			}
			num += nitems.ToInt32();
			XplatUIX11.XFree(prop);
		}
		while (bytes_after.ToInt32() > 0);
		return memoryStream;
	}

	private Control MwfWindow(IntPtr window)
	{
		Hwnd hwnd = Hwnd.ObjectFromHandle(window);
		if (hwnd == null)
		{
			return null;
		}
		Control control = Control.FromHandle(hwnd.client_window);
		if (control == null)
		{
			control = Control.FromHandle(window);
		}
		return control;
	}

	private bool IsWindowDndAware(IntPtr handle)
	{
		bool result = true;
		IntPtr prop = IntPtr.Zero;
		XplatUIX11.XGetWindowProperty(display, handle, XdndAware, IntPtr.Zero, new IntPtr(134217728), delete: false, (IntPtr)4, out var actual_type, out var actual_format, out var nitems, out var _, ref prop);
		if (actual_type != (IntPtr)4 || actual_format != 32 || nitems.ToInt32() == 0 || prop == IntPtr.Zero)
		{
			if (prop != IntPtr.Zero)
			{
				XplatUIX11.XFree(prop);
			}
			return false;
		}
		int num = Marshal.ReadInt32(prop, 0);
		if (num < 3)
		{
			Console.Error.WriteLine("XDND Version too old (" + num + ").");
			XplatUIX11.XFree(prop);
			return false;
		}
		if (nitems.ToInt32() > 1)
		{
			result = false;
			for (int i = 1; i < nitems.ToInt32(); i++)
			{
				IntPtr intPtr = (IntPtr)Marshal.ReadInt32(prop, i * Marshal.SizeOf(typeof(int)));
				for (int j = 0; j < drag_data.SupportedTypes.Length; j++)
				{
					if (drag_data.SupportedTypes[j] == intPtr)
					{
						result = true;
						break;
					}
				}
			}
		}
		XplatUIX11.XFree(prop);
		return result;
	}

	private IntPtr[] DetermineSupportedTypes(object data)
	{
		ArrayList arrayList = new ArrayList();
		if (data is string)
		{
			MimeHandler mimeHandler = FindHandler("text/plain");
			if (mimeHandler != null)
			{
				arrayList.Add(mimeHandler.Type);
			}
		}
		if (data is IDataObject dataObject)
		{
			string[] formats = dataObject.GetFormats(autoConvert: true);
			foreach (string name in formats)
			{
				MimeHandler mimeHandler2 = FindHandler(name);
				if (mimeHandler2 != null && !arrayList.Contains(mimeHandler2.Type))
				{
					arrayList.Add(mimeHandler2.Type);
				}
			}
		}
		if (data is ISerializable)
		{
			MimeHandler mimeHandler3 = FindHandler("application/x-mono-serialized-object");
			if (mimeHandler3 != null)
			{
				arrayList.Add(mimeHandler3.Type);
			}
		}
		return (IntPtr[])arrayList.ToArray(typeof(IntPtr));
	}
}
