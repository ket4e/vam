using System;
using System.Collections;
using Mono.WebBrowser.DOM;

namespace Mono.Mozilla.DOM;

internal class WindowCollection : DOMObject, IEnumerable, IList, ICollection, IWindowCollection
{
	internal class WindowEnumerator : IEnumerator
	{
		private WindowCollection collection;

		private int index = -1;

		public object Current
		{
			get
			{
				if (index == -1)
				{
					return null;
				}
				return collection[index];
			}
		}

		public WindowEnumerator(WindowCollection collection)
		{
			this.collection = collection;
		}

		public bool MoveNext()
		{
			if (index + 1 >= collection.Count)
			{
				return false;
			}
			index++;
			return true;
		}

		public void Reset()
		{
			index = -1;
		}
	}

	protected nsIDOMWindowCollection unmanagedWindows;

	protected IWindow[] windows;

	protected int windowCount;

	object ICollection.SyncRoot => this;

	bool ICollection.IsSynchronized => false;

	bool IList.IsFixedSize => false;

	object IList.this[int index]
	{
		get
		{
			return this[index];
		}
		set
		{
			this[index] = value as IWindow;
		}
	}

	public int Count
	{
		get
		{
			if (unmanagedWindows != null && windows == null)
			{
				Load();
			}
			return windowCount;
		}
	}

	public bool IsReadOnly => false;

	public IWindow this[int index]
	{
		get
		{
			if (index < 0 || index >= windowCount)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			return windows[index];
		}
		set
		{
			if (index < 0 || index >= windowCount)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			windows[index] = value;
		}
	}

	public WindowCollection(WebBrowser control, nsIDOMWindowCollection windowCol)
		: base(control)
	{
		if (control.platform != control.enginePlatform)
		{
			unmanagedWindows = nsDOMWindowCollection.GetProxy(control, windowCol);
		}
		else
		{
			unmanagedWindows = windowCol;
		}
	}

	public WindowCollection(WebBrowser control)
		: base(control)
	{
		windows = new Window[0];
	}

	void IList.RemoveAt(int index)
	{
		RemoveAt(index);
	}

	void IList.Remove(object window)
	{
		Remove(window as IWindow);
	}

	void IList.Insert(int index, object value)
	{
		Insert(index, value as IWindow);
	}

	int IList.IndexOf(object window)
	{
		return IndexOf(window as IWindow);
	}

	bool IList.Contains(object window)
	{
		return Contains(window as IWindow);
	}

	void IList.Clear()
	{
		Clear();
	}

	int IList.Add(object window)
	{
		return Add(window as IWindow);
	}

	protected override void Dispose(bool disposing)
	{
		if (!disposed && disposing)
		{
			Clear();
		}
		base.Dispose(disposing);
	}

	protected void Clear()
	{
		if (windows != null)
		{
			for (int i = 0; i < windowCount; i++)
			{
				windows[i] = null;
			}
			windowCount = 0;
			windows = null;
		}
	}

	internal void Load()
	{
		Clear();
		unmanagedWindows.getLength(out var ret);
		Window[] array = new Window[ret];
		for (int i = 0; i < ret; i++)
		{
			unmanagedWindows.item((uint)i, out var ret2);
			array[windowCount++] = new Window(control, ret2);
		}
		windows = new Window[windowCount];
		Array.Copy(array, windows, windowCount);
	}

	public IEnumerator GetEnumerator()
	{
		return new WindowEnumerator(this);
	}

	public void CopyTo(Array dest, int index)
	{
		if (windows != null)
		{
			Array.Copy(windows, 0, dest, index, windowCount);
		}
	}

	public void RemoveAt(int index)
	{
		if (index <= windowCount && index >= 0)
		{
			Array.Copy(windows, index + 1, windows, index, windowCount - index - 1);
			windowCount--;
			windows[windowCount] = null;
		}
	}

	public void Remove(IWindow window)
	{
		RemoveAt(IndexOf(window));
	}

	public void Insert(int index, IWindow value)
	{
		if (index > windowCount)
		{
			index = windowCount;
		}
		IWindow[] array = new Window[windowCount + 1];
		if (index > 0)
		{
			Array.Copy(windows, 0, array, 0, index);
		}
		array[index] = value;
		if (index < windowCount)
		{
			Array.Copy(windows, index, array, index + 1, windowCount - index);
		}
		windows = array;
		windowCount++;
	}

	public int IndexOf(IWindow window)
	{
		return Array.IndexOf(windows, window);
	}

	public bool Contains(IWindow window)
	{
		return IndexOf(window) != -1;
	}

	public int Add(IWindow window)
	{
		Insert(windowCount + 1, window);
		return windowCount - 1;
	}

	public override int GetHashCode()
	{
		if (unmanagedWindows != null)
		{
			return unmanagedWindows.GetHashCode();
		}
		return base.GetHashCode();
	}
}
