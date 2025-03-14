using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;
using Mono.WebBrowser.DOM;

namespace Mono.Mozilla.DOM;

internal class Document : Node, IDocument, INode
{
	private EventHandlerList events;

	internal static object LoadStoppedEvent;

	internal new nsIDOMDocument node
	{
		get
		{
			return base.node as nsIDOMDocument;
		}
		set
		{
			base.node = value;
		}
	}

	internal new nsIDOMDocument XPComObject => node;

	public IElement Active
	{
		get
		{
			nsIWebBrowserFocus nsIWebBrowserFocus = (nsIWebBrowserFocus)control.navigation.navigation;
			if (nsIWebBrowserFocus == null)
			{
				return null;
			}
			nsIWebBrowserFocus.getFocusedElement(out var ret);
			return (IElement)GetTypedNode(ret);
		}
	}

	public string ActiveLinkColor
	{
		get
		{
			if (!(node is nsIDOMHTMLDocument))
			{
				return string.Empty;
			}
			((nsIDOMHTMLDocument)node).getBody(out var ret);
			((nsIDOMHTMLBodyElement)ret).getALink(storage);
			return Base.StringGet(storage);
		}
		set
		{
			if (node is nsIDOMHTMLDocument)
			{
				Base.StringSet(storage, value);
				((nsIDOMHTMLDocument)node).getBody(out var ret);
				((nsIDOMHTMLBodyElement)ret).setALink(storage);
			}
		}
	}

	public IElementCollection Anchors
	{
		get
		{
			if (!(node is nsIDOMHTMLDocument))
			{
				return null;
			}
			((nsIDOMHTMLDocument)node).getAnchors(out var ret);
			return new HTMLElementCollection(control, (nsIDOMNodeList)ret);
		}
	}

	public IElementCollection Applets
	{
		get
		{
			if (!(node is nsIDOMHTMLDocument))
			{
				return null;
			}
			((nsIDOMHTMLDocument)node).getApplets(out var ret);
			return new HTMLElementCollection(control, (nsIDOMNodeList)ret);
		}
	}

	public string Background
	{
		get
		{
			if (!(node is nsIDOMHTMLDocument))
			{
				return string.Empty;
			}
			((nsIDOMHTMLDocument)node).getBody(out var ret);
			((nsIDOMHTMLBodyElement)ret).getBackground(storage);
			return Base.StringGet(storage);
		}
		set
		{
			if (node is nsIDOMHTMLDocument)
			{
				Base.StringSet(storage, value);
				((nsIDOMHTMLDocument)node).getBody(out var ret);
				((nsIDOMHTMLBodyElement)ret).setBackground(storage);
			}
		}
	}

	public string BackColor
	{
		get
		{
			if (!(node is nsIDOMHTMLDocument))
			{
				return string.Empty;
			}
			((nsIDOMHTMLDocument)node).getBody(out var ret);
			((nsIDOMHTMLBodyElement)ret).getBgColor(storage);
			return Base.StringGet(storage);
		}
		set
		{
			if (node is nsIDOMHTMLDocument)
			{
				Base.StringSet(storage, value);
				((nsIDOMHTMLDocument)node).getBody(out var ret);
				((nsIDOMHTMLBodyElement)ret).setBgColor(storage);
			}
		}
	}

	public IElement Body
	{
		get
		{
			if (!(node is nsIDOMHTMLDocument))
			{
				return null;
			}
			if (!resources.Contains("Body"))
			{
				((nsIDOMHTMLDocument)node).getBody(out var ret);
				resources.Add("Body", GetTypedNode(ret));
			}
			return resources["Body"] as IElement;
		}
	}

	public string Charset
	{
		get
		{
			nsIDOMDocumentView nsIDOMDocumentView = (nsIDOMDocumentView)node;
			nsIDOMDocumentView.getDefaultView(out var ret);
			nsIInterfaceRequestor nsIInterfaceRequestor = (nsIInterfaceRequestor)ret;
			nsIInterfaceRequestor.getInterface(typeof(nsIDocCharset).GUID, out var result);
			nsIDocCharset nsIDocCharset = (nsIDocCharset)Marshal.GetObjectForIUnknown(result);
			StringBuilder stringBuilder = new StringBuilder(30);
			IntPtr ret2 = Marshal.StringToHGlobalUni(stringBuilder.ToString());
			nsIDocCharset.getCharset(ref ret2);
			return Marshal.PtrToStringAnsi(ret2);
		}
		set
		{
			nsIDOMDocumentView nsIDOMDocumentView = (nsIDOMDocumentView)node;
			nsIDOMDocumentView.getDefaultView(out var ret);
			nsIInterfaceRequestor nsIInterfaceRequestor = (nsIInterfaceRequestor)ret;
			nsIInterfaceRequestor.getInterface(typeof(nsIDocCharset).GUID, out var result);
			nsIDocCharset nsIDocCharset = (nsIDocCharset)Marshal.GetTypedObjectForIUnknown(result, typeof(nsIDocCharset));
			nsIDocCharset.setCharset(value);
			control.navigation.Go(Url, LoadFlags.CharsetChange);
		}
	}

	public string Cookie
	{
		get
		{
			if (!(node is nsIDOMHTMLDocument))
			{
				return string.Empty;
			}
			((nsIDOMHTMLDocument)node).getCookie(storage);
			return Base.StringGet(storage);
		}
		set
		{
			if (node is nsIDOMHTMLDocument)
			{
				Base.StringSet(storage, value);
				((nsIDOMHTMLDocument)node).setCookie(storage);
			}
		}
	}

	public string Domain
	{
		get
		{
			if (!(node is nsIDOMHTMLDocument))
			{
				return string.Empty;
			}
			((nsIDOMHTMLDocument)node).getDomain(storage);
			return Base.StringGet(storage);
		}
	}

	public IElement DocumentElement
	{
		get
		{
			if (!resources.Contains("DocumentElement"))
			{
				node.getDocumentElement(out var ret);
				resources.Add("DocumentElement", GetTypedNode(ret));
			}
			return resources["DocumentElement"] as IElement;
		}
	}

	public IDocumentType DocType
	{
		get
		{
			node.getDoctype(out var ret);
			return new DocumentType(control, ret);
		}
	}

	public string ForeColor
	{
		get
		{
			if (!(node is nsIDOMHTMLDocument))
			{
				return string.Empty;
			}
			((nsIDOMHTMLDocument)node).getBody(out var ret);
			((nsIDOMHTMLBodyElement)ret).getText(storage);
			return Base.StringGet(storage);
		}
		set
		{
			if (node is nsIDOMHTMLDocument)
			{
				Base.StringSet(storage, value);
				((nsIDOMHTMLDocument)node).getBody(out var ret);
				((nsIDOMHTMLBodyElement)ret).setText(storage);
			}
		}
	}

	public IElementCollection Forms
	{
		get
		{
			if (!(node is nsIDOMHTMLDocument))
			{
				return null;
			}
			((nsIDOMHTMLDocument)node).getForms(out var ret);
			return new HTMLElementCollection(control, (nsIDOMNodeList)ret);
		}
	}

	public IElementCollection Images
	{
		get
		{
			if (!(node is nsIDOMHTMLDocument))
			{
				return null;
			}
			((nsIDOMHTMLDocument)node).getImages(out var ret);
			return new HTMLElementCollection(control, (nsIDOMNodeList)ret);
		}
	}

	public IDOMImplementation Implementation
	{
		get
		{
			node.getImplementation(out var ret);
			return new DOMImplementation(control, ret);
		}
	}

	public string LinkColor
	{
		get
		{
			if (!(node is nsIDOMHTMLDocument))
			{
				return string.Empty;
			}
			((nsIDOMHTMLDocument)node).getBody(out var ret);
			((nsIDOMHTMLBodyElement)ret).getLink(storage);
			return Base.StringGet(storage);
		}
		set
		{
			if (node is nsIDOMHTMLDocument)
			{
				Base.StringSet(storage, value);
				((nsIDOMHTMLDocument)node).getBody(out var ret);
				((nsIDOMHTMLBodyElement)ret).setLink(storage);
			}
		}
	}

	public IElementCollection Links
	{
		get
		{
			if (!(node is nsIDOMHTMLDocument))
			{
				return null;
			}
			((nsIDOMHTMLDocument)node).getLinks(out var ret);
			return new HTMLElementCollection(control, (nsIDOMNodeList)ret);
		}
	}

	public IStylesheetList Stylesheets
	{
		get
		{
			nsIDOMDocumentStyle nsIDOMDocumentStyle = (nsIDOMDocumentStyle)node;
			nsIDOMDocumentStyle.getStyleSheets(out var ret);
			return new StylesheetList(control, ret);
		}
	}

	public string Title
	{
		get
		{
			if (!(node is nsIDOMHTMLDocument))
			{
				return string.Empty;
			}
			((nsIDOMHTMLDocument)node).getTitle(storage);
			return Base.StringGet(storage);
		}
		set
		{
			Base.StringSet(storage, value);
			((nsIDOMHTMLDocument)node).setTitle(storage);
		}
	}

	public string Url
	{
		get
		{
			if (!(node is nsIDOMHTMLDocument))
			{
				return string.Empty;
			}
			((nsIDOMHTMLDocument)node).getURL(storage);
			return Base.StringGet(storage);
		}
	}

	public string VisitedLinkColor
	{
		get
		{
			if (!(node is nsIDOMHTMLDocument))
			{
				return string.Empty;
			}
			((nsIDOMHTMLDocument)node).getBody(out var ret);
			((nsIDOMHTMLBodyElement)ret).getVLink(storage);
			return Base.StringGet(storage);
		}
		set
		{
			if (node is nsIDOMHTMLDocument)
			{
				Base.StringSet(storage, value);
				((nsIDOMHTMLDocument)node).getBody(out var ret);
				((nsIDOMHTMLBodyElement)ret).setVLink(storage);
			}
		}
	}

	public IWindow Window
	{
		get
		{
			nsIDOMDocumentView nsIDOMDocumentView = (nsIDOMDocumentView)node;
			nsIDOMDocumentView.getDefaultView(out var ret);
			nsIInterfaceRequestor nsIInterfaceRequestor = (nsIInterfaceRequestor)ret;
			if (nsIInterfaceRequestor == null)
			{
				return null;
			}
			nsIInterfaceRequestor.getInterface(typeof(nsIDOMWindow).GUID, out var result);
			nsIDOMWindow domWindow = (nsIDOMWindow)Marshal.GetObjectForIUnknown(result);
			return new Window(control, domWindow);
		}
	}

	internal new EventHandlerList Events
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

	public event EventHandler LoadStopped
	{
		add
		{
			Events.AddHandler(LoadStoppedEvent, value);
		}
		remove
		{
			Events.RemoveHandler(LoadStoppedEvent, value);
		}
	}

	public Document(WebBrowser control, nsIDOMHTMLDocument document)
		: base(control, document)
	{
		if (control.platform != control.enginePlatform)
		{
			node = nsDOMHTMLDocument.GetProxy(control, document);
		}
		else
		{
			node = document;
		}
	}

	public Document(WebBrowser control, nsIDOMDocument document)
		: base(control, document)
	{
		if (control.platform != control.enginePlatform)
		{
			node = nsDOMDocument.GetProxy(control, document);
		}
		else
		{
			node = document;
		}
	}

	static Document()
	{
		LoadStopped = new object();
	}

	protected override void Dispose(bool disposing)
	{
		if (!disposed && disposing)
		{
			resources.Clear();
			node = null;
		}
		base.Dispose(disposing);
	}

	public IAttribute CreateAttribute(string name)
	{
		Base.StringSet(storage, name);
		node.createAttribute(storage, out var ret);
		return new Attribute(control, ret);
	}

	public IElement CreateElement(string tagName)
	{
		Base.StringSet(storage, tagName);
		node.createElement(storage, out var ret);
		if (node is nsIDOMHTMLDocument)
		{
			return new HTMLElement(control, (nsIDOMHTMLElement)ret);
		}
		return new Element(control, ret);
	}

	public IElement GetElementById(string id)
	{
		if (!resources.Contains("GetElementById" + id))
		{
			Base.StringSet(storage, id);
			node.getElementById(storage, out var ret);
			if (ret == null)
			{
				return null;
			}
			resources.Add("GetElementById" + id, GetTypedNode(ret));
		}
		return resources["GetElementById" + id] as IElement;
	}

	public IElementCollection GetElementsByTagName(string name)
	{
		if (!resources.Contains("GetElementsByTagName" + name))
		{
			node.getElementsByTagName(storage, out var ret);
			if (ret == null)
			{
				return null;
			}
			resources.Add("GetElementsByTagName" + name, new HTMLElementCollection(control, ret));
		}
		return resources["GetElementsByTagName" + name] as IElementCollection;
	}

	public IElement GetElement(int x, int y)
	{
		node.getChildNodes(out var ret);
		HTMLElementCollection hTMLElementCollection = new HTMLElementCollection(control, ret);
		IElement result = null;
		foreach (Element item in hTMLElementCollection)
		{
			if (item.Left <= x && item.Top <= y && item.Left + item.Width >= x && item.Top + item.Height >= y)
			{
				result = item;
				break;
			}
		}
		return result;
	}

	public void Write(string text)
	{
		if (node is nsIDOMHTMLDocument)
		{
			Base.StringSet(storage, text);
			((nsIDOMHTMLDocument)node).write(storage);
		}
	}

	public string InvokeScript(string script)
	{
		return Base.EvalScript(control, script);
	}

	public override int GetHashCode()
	{
		return node.GetHashCode();
	}
}
