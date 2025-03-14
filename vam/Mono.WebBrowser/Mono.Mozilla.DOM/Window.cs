using System;
using Mono.WebBrowser.DOM;

namespace Mono.Mozilla.DOM;

internal class Window : DOMObject, IWindow
{
	internal nsIDOMWindow window;

	private EventListener eventListener;

	private int hashcode;

	private static object LoadEvent;

	private static object UnloadEvent;

	public IDocument Document
	{
		get
		{
			window.getDocument(out var ret);
			if (!control.documents.ContainsKey(ret.GetHashCode()))
			{
				control.documents.Add(ret.GetHashCode(), new Document(control, (nsIDOMHTMLDocument)ret));
			}
			return control.documents[ret.GetHashCode()] as IDocument;
		}
	}

	public IWindowCollection Frames
	{
		get
		{
			window.getFrames(out var ret);
			return new WindowCollection(control, ret);
		}
	}

	public string Name
	{
		get
		{
			window.getName(storage);
			return Base.StringGet(storage);
		}
		set
		{
			Base.StringSet(storage, value);
			window.setName(storage);
		}
	}

	public IWindow Parent
	{
		get
		{
			window.getParent(out var ret);
			return new Window(control, ret);
		}
	}

	public IWindow Top
	{
		get
		{
			window.getTop(out var ret);
			return new Window(control, ret);
		}
	}

	public string StatusText => control.StatusText;

	public IHistory History
	{
		get
		{
			Navigation navigation = new Navigation(control, window as nsIWebNavigation);
			return new History(control, navigation);
		}
	}

	private EventListener EventListener
	{
		get
		{
			if (eventListener == null)
			{
				eventListener = new EventListener(window as nsIDOMEventTarget, this);
			}
			return eventListener;
		}
	}

	public event EventHandler Load
	{
		add
		{
			base.Events.AddHandler(LoadEvent, value);
			AttachEventHandler("load", value);
		}
		remove
		{
			base.Events.RemoveHandler(LoadEvent, value);
			DetachEventHandler("load", value);
		}
	}

	public event EventHandler Unload
	{
		add
		{
			base.Events.AddHandler(UnloadEvent, value);
			AttachEventHandler("unload", value);
		}
		remove
		{
			base.Events.RemoveHandler(UnloadEvent, value);
			DetachEventHandler("unload", value);
		}
	}

	public event EventHandler OnFocus
	{
		add
		{
			AttachEventHandler("focus", value);
		}
		remove
		{
			DetachEventHandler("focus", value);
		}
	}

	public event EventHandler OnBlur
	{
		add
		{
			AttachEventHandler("blur", value);
		}
		remove
		{
			DetachEventHandler("blur", value);
		}
	}

	public event EventHandler Error
	{
		add
		{
			AttachEventHandler("error", value);
		}
		remove
		{
			DetachEventHandler("error", value);
		}
	}

	public event EventHandler Scroll
	{
		add
		{
			AttachEventHandler("scroll", value);
		}
		remove
		{
			DetachEventHandler("scroll", value);
		}
	}

	public Window(WebBrowser control, nsIDOMWindow domWindow)
		: base(control)
	{
		hashcode = domWindow.GetHashCode();
		window = domWindow;
	}

	static Window()
	{
		Load = new object();
		Unload = new object();
	}

	protected override void Dispose(bool disposing)
	{
		if (!disposed && disposing)
		{
			resources.Clear();
			window = null;
		}
		base.Dispose(disposing);
	}

	internal static bool FindDocument(ref nsIDOMWindow window, int docHashcode)
	{
		window.getDocument(out var ret);
		if (ret.GetHashCode() == docHashcode)
		{
			return true;
		}
		uint ret2 = 1u;
		window.getFrames(out var ret3);
		ret3.getLength(out ret2);
		for (uint num = 0u; num < ret2; num++)
		{
			ret3.item(num, out window);
			if (FindDocument(ref window, docHashcode))
			{
				return true;
			}
		}
		return false;
	}

	public void AttachEventHandler(string eventName, EventHandler handler)
	{
		EventListener.AddHandler(handler, eventName);
	}

	public void DetachEventHandler(string eventName, EventHandler handler)
	{
		EventListener.RemoveHandler(handler, eventName);
	}

	public void Focus()
	{
		nsIWebBrowserFocus nsIWebBrowserFocus = (nsIWebBrowserFocus)window;
		nsIWebBrowserFocus.setFocusedWindow(window);
	}

	public void Open(string url)
	{
		nsIWebNavigation nsIWebNavigation = (nsIWebNavigation)window;
		nsIWebNavigation.loadURI(url, 0u, null, null, null);
	}

	public void ScrollTo(int x, int y)
	{
		window.scrollTo(x, y);
	}

	public override bool Equals(object obj)
	{
		return this == obj as Window;
	}

	public override int GetHashCode()
	{
		return hashcode;
	}

	public void OnLoad()
	{
		EventHandler eventHandler = (EventHandler)base.Events[Load];
		if (eventHandler != null)
		{
			EventArgs e = new EventArgs();
			eventHandler(this, e);
		}
	}

	public void OnUnload()
	{
		EventHandler eventHandler = (EventHandler)base.Events[Unload];
		if (eventHandler != null)
		{
			EventArgs e = new EventArgs();
			eventHandler(this, e);
		}
	}

	public static bool operator ==(Window left, Window right)
	{
		if ((object)left == right)
		{
			return true;
		}
		if ((object)left == null || (object)right == null)
		{
			return false;
		}
		return left.hashcode == right.hashcode;
	}

	public static bool operator !=(Window left, Window right)
	{
		return !(left == right);
	}
}
