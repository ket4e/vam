using System;
using System.Collections;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;
using Mono.Mozilla.DOM;
using Mono.WebBrowser;
using Mono.WebBrowser.DOM;

namespace Mono.Mozilla;

internal class WebBrowser : IWebBrowser
{
	private bool loaded;

	internal bool created;

	private bool creating;

	internal Document document;

	internal Navigation navigation;

	internal Platform platform;

	internal Platform enginePlatform;

	internal Callback callbacks;

	private EventHandlerList events;

	private EventHandlerList domEvents;

	private string statusText;

	private bool streamingMode;

	internal Hashtable documents;

	private int width;

	private int height;

	private bool isDirty;

	private ContentListener contentListener;

	private nsIServiceManager servMan;

	private nsIIOService ioService;

	private nsIAccessibilityService accessibilityService;

	private nsIErrorService errorService;

	private DocumentEncoder docEncoder;

	internal static object KeyDownEvent;

	internal static object KeyPressEvent;

	internal static object KeyUpEvent;

	internal static object MouseClickEvent;

	internal static object MouseDoubleClickEvent;

	internal static object MouseDownEvent;

	internal static object MouseEnterEvent;

	internal static object MouseLeaveEvent;

	internal static object MouseMoveEvent;

	internal static object MouseUpEvent;

	internal static object FocusEvent;

	internal static object BlurEvent;

	internal static object CreateNewWindowEvent;

	internal static object AlertEvent;

	internal static object LoadStartedEvent;

	internal static object LoadCommitedEvent;

	internal static object ProgressChangedEvent;

	internal static object LoadFinishedEvent;

	internal static object LoadEvent;

	internal static object UnloadEvent;

	internal static object StatusChangedEvent;

	internal static object SecurityChangedEvent;

	internal static object ProgressEvent;

	internal static object ContextMenuEvent;

	internal static object NavigationRequestedEvent;

	internal static object GenericEvent;

	private bool Created
	{
		get
		{
			if (!creating && !created)
			{
				creating = true;
				created = Base.Create(this);
				if (created && isDirty)
				{
					isDirty = false;
					Base.Resize(this, width, height);
				}
			}
			return created;
		}
	}

	public bool Initialized => loaded;

	public IWindow Window
	{
		get
		{
			if (Navigation != null)
			{
				nsIWebBrowserFocus nsIWebBrowserFocus2 = (nsIWebBrowserFocus)navigation.navigation;
				nsIWebBrowserFocus2.getFocusedWindow(out var ret);
				if (ret == null)
				{
					((nsIWebBrowser)navigation.navigation).getContentDOMWindow(out ret);
				}
				if (ret != null)
				{
					return new Window(this, ret);
				}
			}
			return null;
		}
	}

	public IDocument Document
	{
		get
		{
			if (Navigation != null && document == null)
			{
				document = navigation.Document;
			}
			return document;
		}
	}

	public INavigation Navigation
	{
		get
		{
			if (!Created)
			{
				return null;
			}
			if (navigation == null)
			{
				nsIWebNavigation webNavigation = Base.GetWebNavigation(this);
				navigation = new Navigation(this, webNavigation);
			}
			return navigation;
		}
	}

	public string StatusText => statusText;

	public bool Offline
	{
		get
		{
			if (!Created)
			{
				return true;
			}
			IOService.getOffline(out var ret);
			return ret;
		}
		set
		{
			IOService.setOffline(value);
		}
	}

	internal EventHandlerList DomEvents
	{
		get
		{
			if (domEvents == null)
			{
				domEvents = new EventHandlerList();
			}
			return domEvents;
		}
	}

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

	private ContentListener ContentListener
	{
		get
		{
			if (contentListener == null)
			{
				contentListener = new ContentListener(this);
			}
			return contentListener;
		}
	}

	internal nsIServiceManager ServiceManager
	{
		get
		{
			if (servMan == null)
			{
				servMan = Base.GetServiceManager(this);
			}
			return servMan;
		}
	}

	internal nsIIOService IOService
	{
		get
		{
			if (ioService == null)
			{
				IntPtr ret = IntPtr.Zero;
				ServiceManager.getServiceByContractID("@mozilla.org/network/io-service;1", typeof(nsIIOService).GUID, out ret);
				if (ret == IntPtr.Zero)
				{
					throw new Mono.WebBrowser.Exception(Mono.WebBrowser.Exception.ErrorCodes.IOService);
				}
				try
				{
					ioService = (nsIIOService)Marshal.GetObjectForIUnknown(ret);
				}
				catch (System.Exception innerException)
				{
					throw new Mono.WebBrowser.Exception(Mono.WebBrowser.Exception.ErrorCodes.IOService, innerException);
				}
			}
			return ioService;
		}
	}

	internal nsIAccessibilityService AccessibilityService
	{
		get
		{
			if (accessibilityService == null)
			{
				IntPtr ret = IntPtr.Zero;
				ServiceManager.getServiceByContractID("@mozilla.org/accessibilityService;1", typeof(nsIAccessibilityService).GUID, out ret);
				if (ret == IntPtr.Zero)
				{
					throw new Mono.WebBrowser.Exception(Mono.WebBrowser.Exception.ErrorCodes.AccessibilityService);
				}
				try
				{
					accessibilityService = (nsIAccessibilityService)Marshal.GetObjectForIUnknown(ret);
				}
				catch (System.Exception innerException)
				{
					throw new Mono.WebBrowser.Exception(Mono.WebBrowser.Exception.ErrorCodes.AccessibilityService, innerException);
				}
			}
			return accessibilityService;
		}
	}

	internal nsIErrorService ErrorService
	{
		get
		{
			if (errorService == null)
			{
				IntPtr ret = IntPtr.Zero;
				ServiceManager.getServiceByContractID("@mozilla.org/xpcom/error-service;1", typeof(nsIErrorService).GUID, out ret);
				if (ret == IntPtr.Zero)
				{
					return null;
				}
				try
				{
					errorService = (nsIErrorService)Marshal.GetObjectForIUnknown(ret);
				}
				catch (System.Exception)
				{
					return null;
				}
			}
			return errorService;
		}
	}

	internal DocumentEncoder DocEncoder
	{
		get
		{
			if (docEncoder == null)
			{
				docEncoder = new DocumentEncoder(this);
			}
			return docEncoder;
		}
	}

	public event NodeEventHandler KeyDown
	{
		add
		{
			Events.AddHandler(KeyDownEvent, value);
		}
		remove
		{
			Events.RemoveHandler(KeyDownEvent, value);
		}
	}

	public event NodeEventHandler KeyPress
	{
		add
		{
			Events.AddHandler(KeyPressEvent, value);
		}
		remove
		{
			Events.RemoveHandler(KeyPressEvent, value);
		}
	}

	public event NodeEventHandler KeyUp
	{
		add
		{
			Events.AddHandler(KeyUpEvent, value);
		}
		remove
		{
			Events.RemoveHandler(KeyUpEvent, value);
		}
	}

	public event NodeEventHandler MouseClick
	{
		add
		{
			Events.AddHandler(MouseClickEvent, value);
		}
		remove
		{
			Events.RemoveHandler(MouseClickEvent, value);
		}
	}

	public event NodeEventHandler MouseDoubleClick
	{
		add
		{
			Events.AddHandler(MouseDoubleClickEvent, value);
		}
		remove
		{
			Events.RemoveHandler(MouseDoubleClickEvent, value);
		}
	}

	public event NodeEventHandler MouseDown
	{
		add
		{
			Events.AddHandler(MouseDownEvent, value);
		}
		remove
		{
			Events.RemoveHandler(MouseDownEvent, value);
		}
	}

	public event NodeEventHandler MouseEnter
	{
		add
		{
			Events.AddHandler(MouseEnterEvent, value);
		}
		remove
		{
			Events.RemoveHandler(MouseEnterEvent, value);
		}
	}

	public event NodeEventHandler MouseLeave
	{
		add
		{
			Events.AddHandler(MouseLeaveEvent, value);
		}
		remove
		{
			Events.RemoveHandler(MouseLeaveEvent, value);
		}
	}

	public event NodeEventHandler MouseMove
	{
		add
		{
			Events.AddHandler(MouseMoveEvent, value);
		}
		remove
		{
			Events.RemoveHandler(MouseMoveEvent, value);
		}
	}

	public event NodeEventHandler MouseUp
	{
		add
		{
			Events.AddHandler(MouseUpEvent, value);
		}
		remove
		{
			Events.RemoveHandler(MouseUpEvent, value);
		}
	}

	public event EventHandler Focus
	{
		add
		{
			Events.AddHandler(FocusEvent, value);
		}
		remove
		{
			Events.RemoveHandler(FocusEvent, value);
		}
	}

	public event EventHandler Blur
	{
		add
		{
			Events.AddHandler(BlurEvent, value);
		}
		remove
		{
			Events.RemoveHandler(BlurEvent, value);
		}
	}

	public event CreateNewWindowEventHandler CreateNewWindow
	{
		add
		{
			Events.AddHandler(CreateNewWindowEvent, value);
		}
		remove
		{
			Events.RemoveHandler(CreateNewWindowEvent, value);
		}
	}

	public event AlertEventHandler Alert
	{
		add
		{
			Events.AddHandler(AlertEvent, value);
		}
		remove
		{
			Events.RemoveHandler(AlertEvent, value);
		}
	}

	public event EventHandler Loaded
	{
		add
		{
			Events.AddHandler(LoadEvent, value);
		}
		remove
		{
			Events.RemoveHandler(LoadEvent, value);
		}
	}

	public event EventHandler Unloaded
	{
		add
		{
			Events.AddHandler(UnloadEvent, value);
		}
		remove
		{
			Events.RemoveHandler(UnloadEvent, value);
		}
	}

	public event StatusChangedEventHandler StatusChanged
	{
		add
		{
			Events.AddHandler(StatusChangedEvent, value);
		}
		remove
		{
			Events.RemoveHandler(StatusChangedEvent, value);
		}
	}

	public event SecurityChangedEventHandler SecurityChanged
	{
		add
		{
			Events.AddHandler(SecurityChangedEvent, value);
		}
		remove
		{
			Events.RemoveHandler(SecurityChangedEvent, value);
		}
	}

	public event LoadStartedEventHandler LoadStarted
	{
		add
		{
			Events.AddHandler(LoadStartedEvent, value);
		}
		remove
		{
			Events.RemoveHandler(LoadStartedEvent, value);
		}
	}

	public event LoadCommitedEventHandler LoadCommited
	{
		add
		{
			Events.AddHandler(LoadCommitedEvent, value);
		}
		remove
		{
			Events.RemoveHandler(LoadCommitedEvent, value);
		}
	}

	public event Mono.WebBrowser.ProgressChangedEventHandler ProgressChanged
	{
		add
		{
			Events.AddHandler(ProgressChangedEvent, value);
		}
		remove
		{
			Events.RemoveHandler(ProgressChangedEvent, value);
		}
	}

	public event LoadFinishedEventHandler LoadFinished
	{
		add
		{
			Events.AddHandler(LoadFinishedEvent, value);
		}
		remove
		{
			Events.RemoveHandler(LoadFinishedEvent, value);
		}
	}

	public event ContextMenuEventHandler ContextMenuShown
	{
		add
		{
			Events.AddHandler(ContextMenuEvent, value);
		}
		remove
		{
			Events.RemoveHandler(ContextMenuEvent, value);
		}
	}

	public event NavigationRequestedEventHandler NavigationRequested
	{
		add
		{
			ContentListener.AddHandler(value);
		}
		remove
		{
			ContentListener.RemoveHandler(value);
		}
	}

	internal event EventHandler Generic
	{
		add
		{
			Events.AddHandler(GenericEvent, value);
		}
		remove
		{
			Events.RemoveHandler(GenericEvent, value);
		}
	}

	public WebBrowser(Platform platform)
	{
		this.platform = platform;
		callbacks = new Callback(this);
		loaded = Base.Init(this, platform);
		documents = new Hashtable();
	}

	static WebBrowser()
	{
		KeyDown = new object();
		KeyPress = new object();
		KeyUp = new object();
		MouseClick = new object();
		MouseDoubleClick = new object();
		MouseDown = new object();
		MouseEnter = new object();
		MouseLeave = new object();
		MouseMove = new object();
		MouseUp = new object();
		Focus = new object();
		Blur = new object();
		CreateNewWindow = new object();
		Alert = new object();
		LoadStarted = new object();
		LoadCommited = new object();
		ProgressChanged = new object();
		LoadFinished = new object();
		LoadEvent = new object();
		UnloadEvent = new object();
		StatusChanged = new object();
		SecurityChanged = new object();
		ProgressEvent = new object();
		ContextMenuEvent = new object();
		NavigationRequested = new object();
		Generic = new object();
	}

	public bool Load(IntPtr handle, int width, int height)
	{
		loaded = Base.Bind(this, handle, width, height);
		return loaded;
	}

	public void Shutdown()
	{
		Base.Shutdown(this);
	}

	internal void Reset()
	{
		document = null;
		DomEvents.Dispose();
		domEvents = null;
		documents.Clear();
	}

	public void FocusIn(FocusOption focus)
	{
		if (created)
		{
			Base.Focus(this, focus);
		}
	}

	public void FocusOut()
	{
		if (created)
		{
			Base.Blur(this);
		}
	}

	public void Activate()
	{
		if (Created)
		{
			Base.Activate(this);
		}
	}

	public void Deactivate()
	{
		if (created)
		{
			Base.Deactivate(this);
		}
	}

	public void Resize(int width, int height)
	{
		this.width = width;
		this.height = height;
		isDirty = true;
		if (created)
		{
			Base.Resize(this, width, height);
		}
	}

	public void Render(byte[] data)
	{
		if (Created)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			string @string = Encoding.UTF8.GetString(data);
			Render(@string);
		}
	}

	public void Render(string html)
	{
		if (Created)
		{
			Render(html, "file:///", "text/html");
		}
	}

	public void Render(string html, string uri, string contentType)
	{
		if (Created)
		{
			if (Navigation == null)
			{
				throw new Mono.WebBrowser.Exception(Mono.WebBrowser.Exception.ErrorCodes.Navigation);
			}
			nsIWebBrowserStream nsIWebBrowserStream2 = (nsIWebBrowserStream)navigation.navigation;
			AsciiString asciiString = new AsciiString(uri);
			IOService.newURI(asciiString.Handle, null, null, out var ret);
			AsciiString asciiString2 = new AsciiString(contentType);
			HandleRef handle = asciiString2.Handle;
			nsIWebBrowserStream2.openStream(ret, handle);
			IntPtr intPtr = Marshal.StringToHGlobalAnsi(html);
			nsIWebBrowserStream2.appendToStream(intPtr, (uint)html.Length);
			Marshal.FreeHGlobal(intPtr);
			nsIWebBrowserStream2.closeStream();
		}
	}

	public void ExecuteScript(string script)
	{
		if (Created)
		{
			Base.EvalScript(this, script);
		}
	}

	internal void AttachEvent(INode node, string eve, EventHandler handler)
	{
		string key = string.Intern(node.GetHashCode() + ":" + eve);
		DomEvents.AddHandler(key, handler);
	}

	internal void DetachEvent(INode node, string eve, EventHandler handler)
	{
		string key = string.Intern(node.GetHashCode() + ":" + eve);
		DomEvents.RemoveHandler(key, handler);
	}
}
