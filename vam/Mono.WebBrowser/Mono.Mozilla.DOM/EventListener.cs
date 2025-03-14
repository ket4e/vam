using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Mono.WebBrowser.DOM;

namespace Mono.Mozilla.DOM;

internal class EventListener : nsIDOMEventListener
{
	private HandleRef storage;

	private bool disposed;

	private object owner;

	private EventHandlerList events;

	private nsIDOMEventTarget target;

	public EventHandlerList Events
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

	public nsIDOMEventTarget Target
	{
		get
		{
			return target;
		}
		set
		{
			target = value;
		}
	}

	public EventListener(nsIDOMEventTarget target, object owner)
	{
		this.target = target;
		this.owner = owner;
		IntPtr handle = Base.StringInit();
		storage = new HandleRef(this, handle);
	}

	~EventListener()
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

	public void AddHandler(EventHandler handler, string _event)
	{
		string key = string.Intern(target.GetHashCode() + ":" + _event);
		Events.AddHandler(key, handler);
		Base.StringSet(storage, _event);
		target.addEventListener(storage, this, useCapture: true);
	}

	public void RemoveHandler(EventHandler handler, string _event)
	{
		string key = string.Intern(target.GetHashCode() + ":" + _event);
		Events.RemoveHandler(key, handler);
		Base.StringSet(storage, _event);
		target.removeEventListener(storage, this, useCapture: true);
	}

	public void AddHandler(NodeEventHandler handler, string _event)
	{
		string key = string.Intern(target.GetHashCode() + ":" + _event);
		Events.AddHandler(key, handler);
		Base.StringSet(storage, _event);
		target.addEventListener(storage, this, useCapture: true);
	}

	public void RemoveHandler(NodeEventHandler handler, string _event)
	{
		string key = string.Intern(target.GetHashCode() + ":" + _event);
		Events.RemoveHandler(key, handler);
		Base.StringSet(storage, _event);
		target.removeEventListener(storage, this, useCapture: true);
	}

	public int handleEvent(nsIDOMEvent _event)
	{
		_event.getType(storage);
		string text = Base.StringGet(storage);
		string key = string.Intern(target.GetHashCode() + ":" + text);
		if (Events[key] is EventHandler eventHandler)
		{
			eventHandler(owner, new EventArgs());
			return 0;
		}
		if (Events[key] is NodeEventHandler nodeEventHandler)
		{
			nodeEventHandler(owner, new NodeEventArgs((INode)owner));
			return 0;
		}
		if (Events[key] is WindowEventHandler windowEventHandler)
		{
			windowEventHandler(owner, new WindowEventArgs((IWindow)owner));
			return 0;
		}
		return 0;
	}
}
