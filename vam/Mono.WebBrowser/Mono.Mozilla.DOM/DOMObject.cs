using System;
using System.Collections;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Mono.WebBrowser.DOM;

namespace Mono.Mozilla.DOM;

internal class DOMObject : IDisposable
{
	private EventHandlerList event_handlers;

	protected WebBrowser control;

	internal HandleRef storage;

	protected bool disposed;

	protected Hashtable resources;

	protected EventHandlerList Events
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

	internal DOMObject(WebBrowser control)
	{
		this.control = control;
		IntPtr handle = Base.StringInit();
		storage = new HandleRef(this, handle);
		resources = new Hashtable();
		event_handlers = null;
	}

	~DOMObject()
	{
		Dispose(disposing: false);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (!disposed)
		{
			if (disposing)
			{
				Base.StringFinish(storage);
			}
			disposed = true;
		}
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	internal INode GetTypedNode(nsIDOMNode obj)
	{
		if (obj == null)
		{
			return null;
		}
		obj.getLocalName(storage);
		obj.getNodeType(out var ret);
		switch (ret)
		{
		case 1:
			if (obj is nsIDOMHTMLBodyElement)
			{
				return new HTMLElement(control, obj as nsIDOMHTMLBodyElement);
			}
			if (obj is nsIDOMHTMLStyleElement)
			{
				return new HTMLElement(control, obj as nsIDOMHTMLStyleElement);
			}
			if (obj is nsIDOMHTMLElement)
			{
				return new HTMLElement(control, obj as nsIDOMHTMLElement);
			}
			return new Element(control, obj as nsIDOMElement);
		case 2:
			return new Attribute(control, obj as nsIDOMAttr);
		case 9:
			if (obj is nsIDOMHTMLDocument)
			{
				return new Document(control, obj as nsIDOMHTMLDocument);
			}
			return new Document(control, obj as nsIDOMDocument);
		default:
			return new Node(control, obj);
		}
	}
}
