using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms.WebBrowserDialogs;
using Mono.WebBrowser;
using Mono.WebBrowser.DOM;

namespace System.Windows.Forms;

public sealed class HtmlWindow
{
	private EventHandlerList event_handlers;

	private IWindow window;

	private IWebBrowser webHost;

	private WebBrowser owner;

	private static object ErrorEvent;

	private static object GotFocusEvent;

	private static object LostFocusEvent;

	private static object LoadEvent;

	private static object UnloadEvent;

	private static object ScrollEvent;

	private static object ResizeEvent;

	private EventHandlerList Events
	{
		get
		{
			if (event_handlers == null)
			{
				event_handlers = new EventHandlerList();
			}
			return event_handlers;
		}
	}

	public HtmlDocument Document => new HtmlDocument(owner, webHost, window.Document);

	public object DomWindow
	{
		get
		{
			throw new NotSupportedException("Retrieving a reference to an mshtml interface is not supported. Sorry.");
		}
	}

	public HtmlWindowCollection Frames => new HtmlWindowCollection(owner, webHost, window.Frames);

	public HtmlHistory History => new HtmlHistory(webHost, window.History);

	[System.MonoTODO("Windows are always open")]
	public bool IsClosed => false;

	public string Name
	{
		get
		{
			return window.Name;
		}
		set
		{
			window.Name = value;
		}
	}

	[System.MonoTODO("Separate windows are not supported yet")]
	public HtmlWindow Opener => null;

	public HtmlWindow Parent => new HtmlWindow(owner, webHost, window.Parent);

	public Point Position => owner.Location;

	public Size Size
	{
		get
		{
			return owner.Size;
		}
		set
		{
		}
	}

	public string StatusBarText
	{
		get
		{
			return window.StatusText;
		}
		set
		{
		}
	}

	public HtmlElement WindowFrameElement => new HtmlElement(owner, webHost, window.Document.DocumentElement);

	public Uri Url => Document.Url;

	public event HtmlElementErrorEventHandler Error
	{
		add
		{
			Events.AddHandler(ErrorEvent, value);
			window.Error += OnError;
		}
		remove
		{
			Events.RemoveHandler(ErrorEvent, value);
			window.Error -= OnError;
		}
	}

	public event HtmlElementEventHandler GotFocus
	{
		add
		{
			Events.AddHandler(GotFocusEvent, value);
			window.OnFocus += OnGotFocus;
		}
		remove
		{
			Events.RemoveHandler(GotFocusEvent, value);
			window.OnFocus -= OnGotFocus;
		}
	}

	public event HtmlElementEventHandler LostFocus
	{
		add
		{
			Events.AddHandler(LostFocusEvent, value);
			window.OnBlur += OnLostFocus;
		}
		remove
		{
			Events.RemoveHandler(LostFocusEvent, value);
			window.OnBlur -= OnLostFocus;
		}
	}

	public event HtmlElementEventHandler Load
	{
		add
		{
			Events.AddHandler(LoadEvent, value);
			window.Load += OnLoad;
		}
		remove
		{
			Events.RemoveHandler(LoadEvent, value);
			window.Load -= OnLoad;
		}
	}

	public event HtmlElementEventHandler Unload
	{
		add
		{
			Events.AddHandler(UnloadEvent, value);
			window.Unload += OnUnload;
		}
		remove
		{
			Events.RemoveHandler(UnloadEvent, value);
			window.Unload -= OnUnload;
		}
	}

	public event HtmlElementEventHandler Scroll
	{
		add
		{
			Events.AddHandler(ScrollEvent, value);
			window.Scroll += OnScroll;
		}
		remove
		{
			Events.RemoveHandler(ScrollEvent, value);
			window.Scroll -= OnScroll;
		}
	}

	public event HtmlElementEventHandler Resize
	{
		add
		{
			Events.AddHandler(ResizeEvent, value);
		}
		remove
		{
			Events.RemoveHandler(ResizeEvent, value);
		}
	}

	internal HtmlWindow(WebBrowser owner, IWebBrowser webHost, IWindow iWindow)
	{
		window = iWindow;
		this.webHost = webHost;
		this.owner = owner;
		window.Load += OnLoad;
		window.Unload += OnUnload;
	}

	static HtmlWindow()
	{
		Error = new object();
		GotFocus = new object();
		LostFocus = new object();
		Load = new object();
		Unload = new object();
		Scroll = new object();
		Resize = new object();
	}

	public void Alert(string message)
	{
		MessageBox.Show("Alert", message);
	}

	public bool Confirm(string message)
	{
		DialogResult dialogResult = MessageBox.Show(message, "Confirm", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
		return dialogResult == DialogResult.OK;
	}

	public string Prompt(string message, string defaultInputValue)
	{
		Prompt prompt = new Prompt("Prompt", message, defaultInputValue);
		prompt.Show();
		return prompt.Text;
	}

	public void Navigate(string urlString)
	{
		webHost.Navigation.Go(urlString);
	}

	public void Navigate(Uri url)
	{
		webHost.Navigation.Go(url.ToString());
	}

	public void ScrollTo(Point point)
	{
		ScrollTo(point.X, point.Y);
	}

	public void ScrollTo(int x, int y)
	{
		window.ScrollTo(x, y);
	}

	[System.MonoTODO("Blank opens in current window at the moment. Missing media and search implementations. No options implemented")]
	public HtmlWindow Open(Uri url, string target, string windowOptions, bool replaceEntry)
	{
		return Open(url.ToString(), target, windowOptions, replaceEntry);
	}

	[System.MonoTODO("Blank opens in current window at the moment. Missing media and search implementations. No options implemented")]
	public HtmlWindow Open(string urlString, string target, string windowOptions, bool replaceEntry)
	{
		switch (target)
		{
		case "_blank":
			window.Open(urlString);
			break;
		case "_parent":
			window.Parent.Open(urlString);
			break;
		case "_self":
			window.Open(urlString);
			break;
		case "_top":
			window.Top.Open(urlString);
			break;
		}
		return this;
	}

	[System.MonoTODO("Opens in current window at the moment.")]
	public HtmlWindow OpenNew(string urlString, string windowOptions)
	{
		return Open(urlString, "_blank", windowOptions, replaceEntry: false);
	}

	[System.MonoTODO("Opens in current window at the moment.")]
	public HtmlWindow OpenNew(Uri url, string windowOptions)
	{
		return OpenNew(url.ToString(), windowOptions);
	}

	public void AttachEventHandler(string eventName, EventHandler eventHandler)
	{
		window.AttachEventHandler(eventName, eventHandler);
	}

	public void Close()
	{
		throw new NotImplementedException();
	}

	public void DetachEventHandler(string eventName, EventHandler eventHandler)
	{
		window.DetachEventHandler(eventName, eventHandler);
	}

	public void Focus()
	{
		window.Focus();
	}

	public void MoveTo(Point point)
	{
		throw new NotImplementedException();
	}

	public void MoveTo(int x, int y)
	{
		throw new NotImplementedException();
	}

	public void RemoveFocus()
	{
		webHost.FocusOut();
	}

	public void ResizeTo(Size size)
	{
		throw new NotImplementedException();
	}

	public void ResizeTo(int width, int height)
	{
		throw new NotImplementedException();
	}

	internal void OnError(object sender, EventArgs ev)
	{
		HtmlElementErrorEventHandler htmlElementErrorEventHandler = (HtmlElementErrorEventHandler)Events[Error];
		if (htmlElementErrorEventHandler != null)
		{
			HtmlElementErrorEventArgs e = new HtmlElementErrorEventArgs(string.Empty, 0, null);
			htmlElementErrorEventHandler(this, e);
		}
	}

	internal void OnGotFocus(object sender, EventArgs ev)
	{
		HtmlElementEventHandler htmlElementEventHandler = (HtmlElementEventHandler)Events[GotFocus];
		if (htmlElementEventHandler != null)
		{
			HtmlElementEventArgs e = new HtmlElementEventArgs();
			htmlElementEventHandler(this, e);
		}
	}

	internal void OnLostFocus(object sender, EventArgs ev)
	{
		HtmlElementEventHandler htmlElementEventHandler = (HtmlElementEventHandler)Events[LostFocus];
		if (htmlElementEventHandler != null)
		{
			HtmlElementEventArgs e = new HtmlElementEventArgs();
			htmlElementEventHandler(this, e);
		}
	}

	internal void OnLoad(object sender, EventArgs ev)
	{
		HtmlElementEventHandler htmlElementEventHandler = (HtmlElementEventHandler)Events[Load];
		if (htmlElementEventHandler != null)
		{
			HtmlElementEventArgs e = new HtmlElementEventArgs();
			htmlElementEventHandler(this, e);
		}
	}

	internal void OnUnload(object sender, EventArgs ev)
	{
		HtmlElementEventHandler htmlElementEventHandler = (HtmlElementEventHandler)Events[Unload];
		if (htmlElementEventHandler != null)
		{
			HtmlElementEventArgs e = new HtmlElementEventArgs();
			htmlElementEventHandler(this, e);
		}
	}

	internal void OnScroll(object sender, EventArgs ev)
	{
		HtmlElementEventHandler htmlElementEventHandler = (HtmlElementEventHandler)Events[Scroll];
		if (htmlElementEventHandler != null)
		{
			HtmlElementEventArgs e = new HtmlElementEventArgs();
			htmlElementEventHandler(this, e);
		}
	}

	internal void OnResize(object sender, EventArgs ev)
	{
		HtmlElementEventHandler htmlElementEventHandler = (HtmlElementEventHandler)Events[Resize];
		if (htmlElementEventHandler != null)
		{
			HtmlElementEventArgs e = new HtmlElementEventArgs();
			htmlElementEventHandler(this, e);
		}
	}

	public override int GetHashCode()
	{
		if (window == null)
		{
			return 0;
		}
		return window.GetHashCode();
	}

	public override bool Equals(object obj)
	{
		return this == (HtmlWindow)obj;
	}

	public static bool operator ==(HtmlWindow left, HtmlWindow right)
	{
		if ((object)left == right)
		{
			return true;
		}
		if ((object)left == null || (object)right == null)
		{
			return false;
		}
		return left.window.Equals(right.window);
	}

	public static bool operator !=(HtmlWindow left, HtmlWindow right)
	{
		return !(left == right);
	}
}
