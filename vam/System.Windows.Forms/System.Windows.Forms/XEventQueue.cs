using System.Collections;
using System.Threading;

namespace System.Windows.Forms;

internal class XEventQueue
{
	public class PaintQueue
	{
		private ArrayList hwnds;

		private XEvent xevent;

		public int Count => hwnds.Count;

		public PaintQueue(int size)
		{
			hwnds = new ArrayList(size);
			xevent = default(XEvent);
			xevent.AnyEvent.type = XEventName.Expose;
		}

		public void Enqueue(Hwnd hwnd)
		{
			hwnds.Add(hwnd);
		}

		public void Remove(Hwnd hwnd)
		{
			if (!hwnd.expose_pending && !hwnd.nc_expose_pending)
			{
				hwnds.Remove(hwnd);
			}
		}

		public XEvent Dequeue()
		{
			if (hwnds.Count == 0)
			{
				xevent.ExposeEvent.window = IntPtr.Zero;
				return xevent;
			}
			IEnumerator enumerator = hwnds.GetEnumerator();
			enumerator.MoveNext();
			Hwnd hwnd = (Hwnd)enumerator.Current;
			if (!hwnd.nc_expose_pending || !hwnd.expose_pending)
			{
				hwnds.Remove(hwnd);
			}
			if (hwnd.expose_pending)
			{
				xevent.ExposeEvent.window = hwnd.client_window;
				return xevent;
			}
			xevent.ExposeEvent.window = hwnd.whole_window;
			xevent.ExposeEvent.x = hwnd.nc_invalid.X;
			xevent.ExposeEvent.y = hwnd.nc_invalid.Y;
			xevent.ExposeEvent.width = hwnd.nc_invalid.Width;
			xevent.ExposeEvent.height = hwnd.nc_invalid.Height;
			return xevent;
		}
	}

	private class XQueue
	{
		private XEvent[] xevents;

		private int head;

		private int tail;

		private int size;

		public int Count => size;

		public XQueue(int size)
		{
			xevents = new XEvent[size];
		}

		public void Enqueue(XEvent xevent)
		{
			if (size == xevents.Length)
			{
				Grow();
			}
			xevents[tail] = xevent;
			tail = (tail + 1) % xevents.Length;
			size++;
		}

		public XEvent Dequeue()
		{
			if (size < 1)
			{
				throw new Exception("Attempt to dequeue empty queue.");
			}
			XEvent result = xevents[head];
			head = (head + 1) % xevents.Length;
			size--;
			return result;
		}

		public XEvent Peek()
		{
			if (size < 1)
			{
				throw new Exception("Attempt to peek at empty queue");
			}
			return xevents[head];
		}

		private void Grow()
		{
			int num = xevents.Length * 2;
			XEvent[] array = new XEvent[num];
			xevents.CopyTo(array, 0);
			xevents = array;
			head = 0;
			tail = head + size;
		}
	}

	private XQueue xqueue;

	private XQueue lqueue;

	private PaintQueue paint;

	internal ArrayList timer_list;

	private Thread thread;

	private bool dispatch_idle;

	private static readonly int InitialXEventSize = 100;

	private static readonly int InitialLXEventSize = 10;

	private static readonly int InitialPaintSize = 50;

	public int Count
	{
		get
		{
			lock (lqueue)
			{
				return xqueue.Count + lqueue.Count;
			}
		}
	}

	public PaintQueue Paint => paint;

	public Thread Thread => thread;

	public bool DispatchIdle
	{
		get
		{
			return dispatch_idle;
		}
		set
		{
			dispatch_idle = value;
		}
	}

	public XEventQueue(Thread thread)
	{
		xqueue = new XQueue(InitialXEventSize);
		lqueue = new XQueue(InitialLXEventSize);
		paint = new PaintQueue(InitialPaintSize);
		timer_list = new ArrayList();
		this.thread = thread;
		dispatch_idle = true;
	}

	public void Enqueue(XEvent xevent)
	{
		if (Thread.CurrentThread != thread)
		{
			Console.WriteLine("Hwnd.Queue.Enqueue called from a different thread without locking.");
			Console.WriteLine(Environment.StackTrace);
		}
		xqueue.Enqueue(xevent);
	}

	public void EnqueueLocked(XEvent xevent)
	{
		lock (lqueue)
		{
			lqueue.Enqueue(xevent);
		}
	}

	public XEvent Dequeue()
	{
		if (Thread.CurrentThread != thread)
		{
			Console.WriteLine("Hwnd.Queue.Dequeue called from a different thread without locking.");
			Console.WriteLine(Environment.StackTrace);
		}
		if (xqueue.Count == 0)
		{
			lock (lqueue)
			{
				return lqueue.Dequeue();
			}
		}
		return xqueue.Dequeue();
	}

	public XEvent Peek()
	{
		if (Thread.CurrentThread != thread)
		{
			Console.WriteLine("Hwnd.Queue.Peek called from a different thread without locking.");
			Console.WriteLine(Environment.StackTrace);
		}
		if (xqueue.Count == 0)
		{
			lock (lqueue)
			{
				return lqueue.Peek();
			}
		}
		return xqueue.Peek();
	}
}
