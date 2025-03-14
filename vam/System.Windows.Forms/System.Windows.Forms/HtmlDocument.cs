using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using Mono.WebBrowser;
using Mono.WebBrowser.DOM;

namespace System.Windows.Forms;

public sealed class HtmlDocument
{
	private EventHandlerList events;

	private IWebBrowser webHost;

	private IDocument document;

	private WebBrowser owner;

	private static object ClickEvent;

	private static object ContextMenuShowingEvent;

	private static object FocusingEvent;

	private static object LosingFocusEvent;

	private static object MouseDownEvent;

	private static object MouseLeaveEvent;

	private static object MouseMoveEvent;

	private static object MouseOverEvent;

	private static object MouseUpEvent;

	private static object StopEvent;

	internal EventHandlerList Events
	{
		get
		{
			if (events == null)
			{
				events = new EventHandlerList();
			}
			return events;
		}
	}

	public HtmlElement ActiveElement
	{
		get
		{
			IElement active = document.Active;
			if (active == null)
			{
				return null;
			}
			return new HtmlElement(owner, webHost, active);
		}
	}

	public Color ActiveLinkColor
	{
		get
		{
			return ParseColor(document.ActiveLinkColor);
		}
		set
		{
			document.ActiveLinkColor = value.ToArgb().ToString();
		}
	}

	public HtmlElementCollection All => new HtmlElementCollection(owner, webHost, document.DocumentElement.All);

	public Color BackColor
	{
		get
		{
			return ParseColor(document.BackColor);
		}
		set
		{
			document.BackColor = value.ToArgb().ToString();
		}
	}

	public HtmlElement Body => new HtmlElement(owner, webHost, document.Body);

	public string Cookie
	{
		get
		{
			return document.Cookie;
		}
		set
		{
			document.Cookie = value;
		}
	}

	public string DefaultEncoding => document.Charset;

	public string Domain
	{
		get
		{
			return document.Domain;
		}
		set
		{
			throw new NotSupportedException("Setting the domain is not supported per the DOM Level 2 HTML specification. Sorry.");
		}
	}

	public object DomDocument
	{
		get
		{
			throw new NotSupportedException("Retrieving a reference to an mshtml interface is not supported. Sorry.");
		}
	}

	public string Encoding
	{
		get
		{
			return document.Charset;
		}
		set
		{
			document.Charset = value;
		}
	}

	public bool Focused => webHost.Window.Document == document;

	public Color ForeColor
	{
		get
		{
			return ParseColor(document.ForeColor);
		}
		set
		{
			document.ForeColor = value.ToArgb().ToString();
		}
	}

	public HtmlElementCollection Forms => new HtmlElementCollection(owner, webHost, document.Forms);

	public HtmlElementCollection Images => new HtmlElementCollection(owner, webHost, document.Images);

	public Color LinkColor
	{
		get
		{
			return ParseColor(document.LinkColor);
		}
		set
		{
			document.LinkColor = value.ToArgb().ToString();
		}
	}

	public HtmlElementCollection Links => new HtmlElementCollection(owner, webHost, document.Links);

	public bool RightToLeft
	{
		get
		{
			IAttribute attribute = document.Attributes["dir"];
			return attribute != null && attribute.Value == "rtl";
		}
		set
		{
			IAttribute attribute = document.Attributes["dir"];
			if (attribute == null && value)
			{
				IAttribute attribute2 = document.CreateAttribute("dir");
				attribute2.Value = "rtl";
				document.AppendChild(attribute2);
			}
			else if (attribute != null && !value)
			{
				document.RemoveChild(attribute);
			}
		}
	}

	public string Title
	{
		get
		{
			if (document == null)
			{
				return string.Empty;
			}
			return document.Title;
		}
		set
		{
			document.Title = value;
		}
	}

	public Uri Url => new Uri(document.Url);

	public Color VisitedLinkColor
	{
		get
		{
			return ParseColor(document.VisitedLinkColor);
		}
		set
		{
			document.VisitedLinkColor = value.ToArgb().ToString();
		}
	}

	public HtmlWindow Window => new HtmlWindow(owner, webHost, webHost.Window);

	internal string DocType
	{
		get
		{
			if (document == null)
			{
				return string.Empty;
			}
			if (document.DocType != null)
			{
				return document.DocType.Name;
			}
			return string.Empty;
		}
	}

	public event HtmlElementEventHandler Click
	{
		add
		{
			Events.AddHandler(ClickEvent, value);
			document.Click += OnClick;
		}
		remove
		{
			Events.RemoveHandler(ClickEvent, value);
			document.Click -= OnClick;
		}
	}

	public event HtmlElementEventHandler ContextMenuShowing
	{
		add
		{
			Events.AddHandler(ContextMenuShowingEvent, value);
			owner.WebHost.ContextMenuShown += OnContextMenuShowing;
		}
		remove
		{
			Events.RemoveHandler(ContextMenuShowingEvent, value);
			owner.WebHost.ContextMenuShown -= OnContextMenuShowing;
		}
	}

	public event HtmlElementEventHandler Focusing
	{
		add
		{
			Events.AddHandler(FocusingEvent, value);
			document.OnFocus += OnFocusing;
		}
		remove
		{
			Events.RemoveHandler(FocusingEvent, value);
			document.OnFocus -= OnFocusing;
		}
	}

	public event HtmlElementEventHandler LosingFocus
	{
		add
		{
			Events.AddHandler(LosingFocusEvent, value);
			document.OnBlur += OnLosingFocus;
		}
		remove
		{
			Events.RemoveHandler(LosingFocusEvent, value);
			document.OnBlur -= OnLosingFocus;
		}
	}

	public event HtmlElementEventHandler MouseDown
	{
		add
		{
			Events.AddHandler(MouseDownEvent, value);
			document.MouseDown += OnMouseDown;
		}
		remove
		{
			Events.RemoveHandler(MouseDownEvent, value);
			document.MouseDown -= OnMouseDown;
		}
	}

	public event HtmlElementEventHandler MouseLeave
	{
		add
		{
			Events.AddHandler(MouseLeaveEvent, value);
			document.MouseLeave += OnMouseLeave;
		}
		remove
		{
			Events.RemoveHandler(MouseLeaveEvent, value);
			document.MouseLeave -= OnMouseLeave;
		}
	}

	public event HtmlElementEventHandler MouseMove
	{
		add
		{
			Events.AddHandler(MouseMoveEvent, value);
			document.MouseMove += OnMouseMove;
		}
		remove
		{
			Events.RemoveHandler(MouseMoveEvent, value);
			document.MouseMove -= OnMouseMove;
		}
	}

	public event HtmlElementEventHandler MouseOver
	{
		add
		{
			Events.AddHandler(MouseOverEvent, value);
			document.MouseOver += OnMouseOver;
		}
		remove
		{
			Events.RemoveHandler(MouseOverEvent, value);
			document.MouseOver -= OnMouseOver;
		}
	}

	public event HtmlElementEventHandler MouseUp
	{
		add
		{
			Events.AddHandler(MouseUpEvent, value);
			document.MouseUp += OnMouseUp;
		}
		remove
		{
			Events.RemoveHandler(MouseUpEvent, value);
			document.MouseUp -= OnMouseUp;
		}
	}

	public event HtmlElementEventHandler Stop
	{
		add
		{
			Events.AddHandler(StopEvent, value);
			document.LoadStopped += OnStop;
		}
		remove
		{
			Events.RemoveHandler(StopEvent, value);
			document.LoadStopped -= OnStop;
		}
	}

	internal HtmlDocument(WebBrowser owner, IWebBrowser webHost)
		: this(owner, webHost, webHost.Document)
	{
	}

	internal HtmlDocument(WebBrowser owner, IWebBrowser webHost, IDocument doc)
	{
		this.webHost = webHost;
		document = doc;
		this.owner = owner;
	}

	static HtmlDocument()
	{
		Click = new object();
		ContextMenuShowing = new object();
		Focusing = new object();
		LosingFocus = new object();
		MouseDown = new object();
		MouseLeave = new object();
		MouseMove = new object();
		MouseOver = new object();
		MouseUp = new object();
		Stop = new object();
	}

	public void AttachEventHandler(string eventName, EventHandler eventHandler)
	{
		document.AttachEventHandler(eventName, eventHandler);
	}

	public HtmlElement CreateElement(string elementTag)
	{
		IElement element = document.CreateElement(elementTag);
		return new HtmlElement(owner, webHost, element);
	}

	public void DetachEventHandler(string eventName, EventHandler eventHandler)
	{
		document.DetachEventHandler(eventName, eventHandler);
	}

	public override bool Equals(object obj)
	{
		return this == (HtmlDocument)obj;
	}

	public void ExecCommand(string command, bool showUI, object value)
	{
		throw new NotImplementedException("Not Supported");
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public void Focus()
	{
		webHost.FocusIn(FocusOption.None);
	}

	public HtmlElement GetElementById(string id)
	{
		IElement elementById = document.GetElementById(id);
		if (elementById != null)
		{
			return new HtmlElement(owner, webHost, elementById);
		}
		return null;
	}

	public HtmlElement GetElementFromPoint(Point point)
	{
		IElement element = document.GetElement(point.X, point.Y);
		if (element != null)
		{
			return new HtmlElement(owner, webHost, element);
		}
		return null;
	}

	public HtmlElementCollection GetElementsByTagName(string tagName)
	{
		IElementCollection elementsByTagName = document.GetElementsByTagName(tagName);
		if (elementsByTagName != null)
		{
			return new HtmlElementCollection(owner, webHost, elementsByTagName);
		}
		return null;
	}

	public override int GetHashCode()
	{
		if (document == null)
		{
			return 0;
		}
		return document.GetHashCode();
	}

	public object InvokeScript(string scriptName)
	{
		return document.InvokeScript("eval ('" + scriptName + "()');");
	}

	public object InvokeScript(string scriptName, object[] args)
	{
		string[] array = new string[args.Length];
		for (int i = 0; i < args.Length; i++)
		{
			if (args[i] is string)
			{
				array[i] = "\"" + args[i].ToString() + "\"";
			}
			else
			{
				array[i] = args[i].ToString();
			}
		}
		return document.InvokeScript("eval ('" + scriptName + "(" + string.Join(",", array) + ")');");
	}

	public HtmlDocument OpenNew(bool replaceInHistory)
	{
		LoadFlags loadFlags = LoadFlags.None;
		if (replaceInHistory)
		{
			loadFlags |= LoadFlags.ReplaceHistory;
		}
		webHost.Navigation.Go("about:blank", loadFlags);
		return this;
	}

	public void Write(string text)
	{
		document.Write(text);
	}

	private void OnClick(object sender, EventArgs e)
	{
		HtmlElementEventHandler htmlElementEventHandler = (HtmlElementEventHandler)Events[Click];
		if (htmlElementEventHandler != null)
		{
			HtmlElementEventArgs e2 = new HtmlElementEventArgs();
			htmlElementEventHandler(this, e2);
		}
	}

	private void OnContextMenuShowing(object sender, ContextMenuEventArgs e)
	{
		HtmlElementEventHandler htmlElementEventHandler = (HtmlElementEventHandler)Events[ContextMenuShowing];
		if (htmlElementEventHandler != null)
		{
			HtmlElementEventArgs htmlElementEventArgs = new HtmlElementEventArgs();
			htmlElementEventHandler(this, htmlElementEventArgs);
			if (htmlElementEventArgs.ReturnValue)
			{
				owner.OnWebHostContextMenuShown(sender, e);
			}
		}
	}

	private void OnFocusing(object sender, EventArgs e)
	{
		HtmlElementEventHandler htmlElementEventHandler = (HtmlElementEventHandler)Events[Focusing];
		if (htmlElementEventHandler != null)
		{
			HtmlElementEventArgs e2 = new HtmlElementEventArgs();
			htmlElementEventHandler(this, e2);
		}
	}

	private void OnLosingFocus(object sender, EventArgs e)
	{
		HtmlElementEventHandler htmlElementEventHandler = (HtmlElementEventHandler)Events[LosingFocus];
		if (htmlElementEventHandler != null)
		{
			HtmlElementEventArgs e2 = new HtmlElementEventArgs();
			htmlElementEventHandler(this, e2);
		}
	}

	private void OnMouseDown(object sender, EventArgs e)
	{
		HtmlElementEventHandler htmlElementEventHandler = (HtmlElementEventHandler)Events[MouseDown];
		if (htmlElementEventHandler != null)
		{
			HtmlElementEventArgs e2 = new HtmlElementEventArgs();
			htmlElementEventHandler(this, e2);
		}
	}

	private void OnMouseLeave(object sender, EventArgs e)
	{
		HtmlElementEventHandler htmlElementEventHandler = (HtmlElementEventHandler)Events[MouseLeave];
		if (htmlElementEventHandler != null)
		{
			HtmlElementEventArgs e2 = new HtmlElementEventArgs();
			htmlElementEventHandler(this, e2);
		}
	}

	private void OnMouseMove(object sender, EventArgs e)
	{
		HtmlElementEventHandler htmlElementEventHandler = (HtmlElementEventHandler)Events[MouseMove];
		if (htmlElementEventHandler != null)
		{
			HtmlElementEventArgs e2 = new HtmlElementEventArgs();
			htmlElementEventHandler(this, e2);
		}
	}

	private void OnMouseOver(object sender, EventArgs e)
	{
		HtmlElementEventHandler htmlElementEventHandler = (HtmlElementEventHandler)Events[MouseOver];
		if (htmlElementEventHandler != null)
		{
			HtmlElementEventArgs e2 = new HtmlElementEventArgs();
			htmlElementEventHandler(this, e2);
		}
	}

	private void OnMouseUp(object sender, EventArgs e)
	{
		HtmlElementEventHandler htmlElementEventHandler = (HtmlElementEventHandler)Events[MouseUp];
		if (htmlElementEventHandler != null)
		{
			HtmlElementEventArgs e2 = new HtmlElementEventArgs();
			htmlElementEventHandler(this, e2);
		}
	}

	private void OnStop(object sender, EventArgs e)
	{
		HtmlElementEventHandler htmlElementEventHandler = (HtmlElementEventHandler)Events[Stop];
		if (htmlElementEventHandler != null)
		{
			HtmlElementEventArgs e2 = new HtmlElementEventArgs();
			htmlElementEventHandler(this, e2);
		}
	}

	private Color ParseColor(string color)
	{
		if (color.IndexOf("#") >= 0)
		{
			return Color.FromArgb(int.Parse(color.Substring(color.IndexOf("#") + 1), NumberStyles.HexNumber));
		}
		return Color.FromName(color);
	}

	public static bool operator ==(HtmlDocument left, HtmlDocument right)
	{
		if ((object)left == right)
		{
			return true;
		}
		if ((object)left == null || (object)right == null)
		{
			return false;
		}
		return left.document.Equals(right.document);
	}

	public static bool operator !=(HtmlDocument left, HtmlDocument right)
	{
		return !(left == right);
	}
}
