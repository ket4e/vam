using System.ComponentModel;
using System.Drawing;
using Mono.WebBrowser;
using Mono.WebBrowser.DOM;

namespace System.Windows.Forms;

public sealed class HtmlElement
{
	private EventHandlerList events;

	private IWebBrowser webHost;

	internal IElement element;

	private WebBrowser owner;

	private static object ClickEvent;

	private static object DoubleClickEvent;

	private static object MouseDownEvent;

	private static object MouseUpEvent;

	private static object MouseMoveEvent;

	private static object MouseOverEvent;

	private static object MouseEnterEvent;

	private static object MouseLeaveEvent;

	private static object KeyDownEvent;

	private static object KeyPressEvent;

	private static object KeyUpEvent;

	private static object DragEvent;

	private static object DragEndEvent;

	private static object DragLeaveEvent;

	private static object DragOverEvent;

	private static object FocusingEvent;

	private static object GotFocusEvent;

	private static object LosingFocusEvent;

	private static object LostFocusEvent;

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

	public HtmlElementCollection All => new HtmlElementCollection(owner, webHost, element.All);

	public bool CanHaveChildren
	{
		get
		{
			string tagName = TagName;
			switch (tagName.ToLowerInvariant())
			{
			case "area":
			case "base":
			case "basefont":
			case "br":
			case "col":
			case "frame":
			case "hr":
			case "img":
			case "input":
			case "isindex":
			case "link":
			case "meta":
			case "param":
				return false;
			default:
				return true;
			}
		}
	}

	public HtmlElementCollection Children => new HtmlElementCollection(owner, webHost, element.Children);

	public Rectangle ClientRectangle => new Rectangle(0, 0, element.ClientWidth, element.ClientHeight);

	public Rectangle OffsetRectangle => new Rectangle(element.OffsetLeft, element.OffsetTop, element.OffsetWidth, element.OffsetHeight);

	public Rectangle ScrollRectangle => new Rectangle(element.ScrollLeft, element.ScrollTop, element.ScrollWidth, element.ScrollHeight);

	public int ScrollLeft
	{
		get
		{
			return element.ScrollLeft;
		}
		set
		{
			element.ScrollLeft = value;
		}
	}

	public int ScrollTop
	{
		get
		{
			return element.ScrollTop;
		}
		set
		{
			element.ScrollTop = value;
		}
	}

	public HtmlElement OffsetParent => new HtmlElement(owner, webHost, element.OffsetParent);

	public HtmlDocument Document => new HtmlDocument(owner, webHost, element.Owner);

	public bool Enabled
	{
		get
		{
			return !element.Disabled;
		}
		set
		{
			element.Disabled = !value;
		}
	}

	public string InnerHtml
	{
		get
		{
			return element.InnerHTML;
		}
		set
		{
			element.InnerHTML = value;
		}
	}

	public string InnerText
	{
		get
		{
			return element.InnerText;
		}
		set
		{
			element.InnerText = value;
		}
	}

	public string Id
	{
		get
		{
			return GetAttribute("id");
		}
		set
		{
			SetAttribute("id", value);
		}
	}

	public string Name
	{
		get
		{
			return GetAttribute("name");
		}
		set
		{
			SetAttribute("name", value);
		}
	}

	public HtmlElement FirstChild => new HtmlElement(owner, webHost, (IElement)element.FirstChild);

	public HtmlElement NextSibling => new HtmlElement(owner, webHost, (IElement)element.Next);

	public HtmlElement Parent => new HtmlElement(owner, webHost, (IElement)element.Parent);

	public string TagName => element.TagName;

	public short TabIndex
	{
		get
		{
			return (short)element.TabIndex;
		}
		set
		{
			element.TabIndex = value;
		}
	}

	public object DomElement
	{
		get
		{
			throw new NotSupportedException("Retrieving a reference to an mshtml interface is not supported. Sorry.");
		}
	}

	public string OuterHtml
	{
		get
		{
			return element.OuterHTML;
		}
		set
		{
			element.OuterHTML = value;
		}
	}

	public string OuterText
	{
		get
		{
			return element.OuterText;
		}
		set
		{
			element.OuterText = value;
		}
	}

	public string Style
	{
		get
		{
			return element.Style;
		}
		set
		{
			element.Style = value;
		}
	}

	public event HtmlElementEventHandler Click
	{
		add
		{
			Events.AddHandler(ClickEvent, value);
			element.Click += OnClick;
		}
		remove
		{
			Events.RemoveHandler(ClickEvent, value);
			element.Click -= OnClick;
		}
	}

	public event HtmlElementEventHandler DoubleClick
	{
		add
		{
			Events.AddHandler(DoubleClickEvent, value);
			element.DoubleClick += OnDoubleClick;
		}
		remove
		{
			Events.RemoveHandler(DoubleClickEvent, value);
			element.DoubleClick -= OnDoubleClick;
		}
	}

	public event HtmlElementEventHandler MouseDown
	{
		add
		{
			Events.AddHandler(MouseDownEvent, value);
			element.MouseDown += OnMouseDown;
		}
		remove
		{
			Events.RemoveHandler(MouseDownEvent, value);
			element.MouseDown -= OnMouseDown;
		}
	}

	public event HtmlElementEventHandler MouseUp
	{
		add
		{
			Events.AddHandler(MouseUpEvent, value);
			element.MouseUp += OnMouseUp;
		}
		remove
		{
			Events.RemoveHandler(MouseUpEvent, value);
			element.MouseUp -= OnMouseUp;
		}
	}

	public event HtmlElementEventHandler MouseMove
	{
		add
		{
			Events.AddHandler(MouseMoveEvent, value);
			element.MouseMove += OnMouseMove;
		}
		remove
		{
			Events.RemoveHandler(MouseMoveEvent, value);
			element.MouseMove -= OnMouseMove;
		}
	}

	public event HtmlElementEventHandler MouseOver
	{
		add
		{
			Events.AddHandler(MouseOverEvent, value);
			element.MouseOver += OnMouseOver;
		}
		remove
		{
			Events.RemoveHandler(MouseOverEvent, value);
			element.MouseOver -= OnMouseOver;
		}
	}

	public event HtmlElementEventHandler MouseEnter
	{
		add
		{
			Events.AddHandler(MouseEnterEvent, value);
			element.MouseEnter += OnMouseEnter;
		}
		remove
		{
			Events.RemoveHandler(MouseEnterEvent, value);
			element.MouseEnter -= OnMouseEnter;
		}
	}

	public event HtmlElementEventHandler MouseLeave
	{
		add
		{
			Events.AddHandler(MouseLeaveEvent, value);
			element.MouseLeave += OnMouseLeave;
		}
		remove
		{
			Events.RemoveHandler(MouseLeaveEvent, value);
			element.MouseLeave -= OnMouseLeave;
		}
	}

	public event HtmlElementEventHandler KeyDown
	{
		add
		{
			Events.AddHandler(KeyDownEvent, value);
			element.KeyDown += OnKeyDown;
		}
		remove
		{
			Events.RemoveHandler(KeyDownEvent, value);
			element.KeyDown -= OnKeyDown;
		}
	}

	public event HtmlElementEventHandler KeyPress
	{
		add
		{
			Events.AddHandler(KeyPressEvent, value);
			element.KeyPress += OnKeyPress;
		}
		remove
		{
			Events.RemoveHandler(KeyPressEvent, value);
			element.KeyPress -= OnKeyPress;
		}
	}

	public event HtmlElementEventHandler KeyUp
	{
		add
		{
			Events.AddHandler(KeyUpEvent, value);
			element.KeyUp += OnKeyUp;
		}
		remove
		{
			Events.RemoveHandler(KeyUpEvent, value);
			element.KeyUp -= OnKeyUp;
		}
	}

	public event HtmlElementEventHandler Drag
	{
		add
		{
			Events.AddHandler(DragEvent, value);
		}
		remove
		{
			Events.RemoveHandler(DragEvent, value);
		}
	}

	public event HtmlElementEventHandler DragEnd
	{
		add
		{
			Events.AddHandler(DragEndEvent, value);
		}
		remove
		{
			Events.RemoveHandler(DragEndEvent, value);
		}
	}

	public event HtmlElementEventHandler DragLeave
	{
		add
		{
			Events.AddHandler(DragLeaveEvent, value);
		}
		remove
		{
			Events.RemoveHandler(DragLeaveEvent, value);
		}
	}

	public event HtmlElementEventHandler DragOver
	{
		add
		{
			Events.AddHandler(DragOverEvent, value);
		}
		remove
		{
			Events.RemoveHandler(DragOverEvent, value);
		}
	}

	public event HtmlElementEventHandler Focusing
	{
		add
		{
			Events.AddHandler(FocusingEvent, value);
			element.OnFocus += OnFocusing;
		}
		remove
		{
			Events.RemoveHandler(FocusingEvent, value);
			element.OnFocus -= OnFocusing;
		}
	}

	public event HtmlElementEventHandler GotFocus
	{
		add
		{
			Events.AddHandler(GotFocusEvent, value);
			element.OnFocus += OnGotFocus;
		}
		remove
		{
			Events.RemoveHandler(GotFocusEvent, value);
			element.OnFocus -= OnGotFocus;
		}
	}

	public event HtmlElementEventHandler LosingFocus
	{
		add
		{
			Events.AddHandler(LosingFocusEvent, value);
			element.OnBlur += OnLosingFocus;
		}
		remove
		{
			Events.RemoveHandler(LosingFocusEvent, value);
			element.OnBlur -= OnLosingFocus;
		}
	}

	public event HtmlElementEventHandler LostFocus
	{
		add
		{
			Events.AddHandler(LostFocusEvent, value);
			element.OnBlur += OnLostFocus;
		}
		remove
		{
			Events.RemoveHandler(LostFocusEvent, value);
			element.OnBlur -= OnLostFocus;
		}
	}

	internal HtmlElement(WebBrowser owner, IWebBrowser webHost, IElement element)
	{
		this.webHost = webHost;
		this.element = element;
		this.owner = owner;
	}

	static HtmlElement()
	{
		Click = new object();
		DoubleClick = new object();
		MouseDown = new object();
		MouseUp = new object();
		MouseMove = new object();
		MouseOver = new object();
		MouseEnter = new object();
		MouseLeave = new object();
		KeyDown = new object();
		KeyPress = new object();
		KeyUp = new object();
		Drag = new object();
		DragEnd = new object();
		DragLeave = new object();
		DragOver = new object();
		Focusing = new object();
		GotFocus = new object();
		LosingFocus = new object();
		LostFocus = new object();
	}

	public HtmlElement AppendChild(HtmlElement newElement)
	{
		IElement element = this.element.AppendChild(newElement.element);
		newElement.element = element;
		return newElement;
	}

	public void AttachEventHandler(string eventName, EventHandler eventHandler)
	{
		element.AttachEventHandler(eventName, eventHandler);
	}

	public void DetachEventHandler(string eventName, EventHandler eventHandler)
	{
		element.DetachEventHandler(eventName, eventHandler);
	}

	public void Focus()
	{
		throw new NotImplementedException();
	}

	public string GetAttribute(string attributeName)
	{
		return element.GetAttribute(attributeName);
	}

	public HtmlElementCollection GetElementsByTagName(string tagName)
	{
		IElementCollection elementsByTagName = element.GetElementsByTagName(tagName);
		return new HtmlElementCollection(owner, webHost, elementsByTagName);
	}

	public override int GetHashCode()
	{
		if (element == null)
		{
			return 0;
		}
		return element.GetHashCode();
	}

	internal bool HasAttribute(string name)
	{
		return element.HasAttribute(name);
	}

	public HtmlElement InsertAdjacentElement(HtmlElementInsertionOrientation orient, HtmlElement newElement)
	{
		switch (orient)
		{
		case HtmlElementInsertionOrientation.BeforeBegin:
			element.Parent.InsertBefore(newElement.element, element);
			return newElement;
		case HtmlElementInsertionOrientation.AfterBegin:
			element.InsertBefore(newElement.element, element.FirstChild);
			return newElement;
		case HtmlElementInsertionOrientation.BeforeEnd:
			return AppendChild(newElement);
		case HtmlElementInsertionOrientation.AfterEnd:
			return AppendChild(newElement);
		default:
			return null;
		}
	}

	public object InvokeMember(string methodName)
	{
		return element.Owner.InvokeScript("eval ('" + methodName + "()');");
	}

	public object InvokeMember(string methodName, params object[] parameter)
	{
		string[] array = new string[parameter.Length];
		for (int i = 0; i < parameter.Length; i++)
		{
			array[i] = parameter.ToString();
		}
		return element.Owner.InvokeScript("eval ('" + methodName + "(" + string.Join(",", array) + ")');");
	}

	public void RaiseEvent(string eventName)
	{
		element.FireEvent(eventName);
	}

	public void RemoveFocus()
	{
		element.Blur();
	}

	public void ScrollIntoView(bool alignWithTop)
	{
		element.ScrollIntoView(alignWithTop);
	}

	public void SetAttribute(string attributeName, string value)
	{
		element.SetAttribute(attributeName, value);
	}

	public override bool Equals(object obj)
	{
		return this == (HtmlElement)obj;
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

	private void OnDoubleClick(object sender, EventArgs e)
	{
		HtmlElementEventHandler htmlElementEventHandler = (HtmlElementEventHandler)Events[DoubleClick];
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

	private void OnMouseUp(object sender, EventArgs e)
	{
		HtmlElementEventHandler htmlElementEventHandler = (HtmlElementEventHandler)Events[MouseUp];
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

	private void OnMouseEnter(object sender, EventArgs e)
	{
		HtmlElementEventHandler htmlElementEventHandler = (HtmlElementEventHandler)Events[MouseEnter];
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

	private void OnKeyDown(object sender, EventArgs e)
	{
		HtmlElementEventHandler htmlElementEventHandler = (HtmlElementEventHandler)Events[KeyDown];
		if (htmlElementEventHandler != null)
		{
			HtmlElementEventArgs e2 = new HtmlElementEventArgs();
			htmlElementEventHandler(this, e2);
		}
	}

	private void OnKeyPress(object sender, EventArgs e)
	{
		HtmlElementEventHandler htmlElementEventHandler = (HtmlElementEventHandler)Events[KeyPress];
		if (htmlElementEventHandler != null)
		{
			HtmlElementEventArgs e2 = new HtmlElementEventArgs();
			htmlElementEventHandler(this, e2);
		}
	}

	private void OnKeyUp(object sender, EventArgs e)
	{
		HtmlElementEventHandler htmlElementEventHandler = (HtmlElementEventHandler)Events[KeyUp];
		if (htmlElementEventHandler != null)
		{
			HtmlElementEventArgs e2 = new HtmlElementEventArgs();
			htmlElementEventHandler(this, e2);
		}
	}

	private void OnDrag(object sender, EventArgs e)
	{
		HtmlElementEventHandler htmlElementEventHandler = (HtmlElementEventHandler)Events[Drag];
		if (htmlElementEventHandler != null)
		{
			HtmlElementEventArgs e2 = new HtmlElementEventArgs();
			htmlElementEventHandler(this, e2);
		}
	}

	private void OnDragEnd(object sender, EventArgs e)
	{
		HtmlElementEventHandler htmlElementEventHandler = (HtmlElementEventHandler)Events[DragEnd];
		if (htmlElementEventHandler != null)
		{
			HtmlElementEventArgs e2 = new HtmlElementEventArgs();
			htmlElementEventHandler(this, e2);
		}
	}

	private void OnDragLeave(object sender, EventArgs e)
	{
		HtmlElementEventHandler htmlElementEventHandler = (HtmlElementEventHandler)Events[DragLeave];
		if (htmlElementEventHandler != null)
		{
			HtmlElementEventArgs e2 = new HtmlElementEventArgs();
			htmlElementEventHandler(this, e2);
		}
	}

	private void OnDragOver(object sender, EventArgs e)
	{
		HtmlElementEventHandler htmlElementEventHandler = (HtmlElementEventHandler)Events[DragOver];
		if (htmlElementEventHandler != null)
		{
			HtmlElementEventArgs e2 = new HtmlElementEventArgs();
			htmlElementEventHandler(this, e2);
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

	private void OnGotFocus(object sender, EventArgs e)
	{
		HtmlElementEventHandler htmlElementEventHandler = (HtmlElementEventHandler)Events[GotFocus];
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

	private void OnLostFocus(object sender, EventArgs e)
	{
		HtmlElementEventHandler htmlElementEventHandler = (HtmlElementEventHandler)Events[LostFocus];
		if (htmlElementEventHandler != null)
		{
			HtmlElementEventArgs e2 = new HtmlElementEventArgs();
			htmlElementEventHandler(this, e2);
		}
	}

	public static bool operator ==(HtmlElement left, HtmlElement right)
	{
		if ((object)left == right)
		{
			return true;
		}
		if ((object)left == null || (object)right == null)
		{
			return false;
		}
		return left.element.Equals(right.element);
	}

	public static bool operator !=(HtmlElement left, HtmlElement right)
	{
		return !(left == right);
	}
}
