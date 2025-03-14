using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Mono.WebBrowser;

namespace Mono.Mozilla.DOM;

internal class ContentListener : nsIURIContentListener
{
	private WebBrowser owner;

	private EventHandlerList events;

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

	public ContentListener(WebBrowser instance)
	{
		owner = instance;
	}

	bool nsIURIContentListener.onStartURIOpen(nsIURI aURI)
	{
		NavigationRequestedEventHandler navigationRequestedEventHandler = (NavigationRequestedEventHandler)Events[WebBrowser.NavigationRequested];
		if (navigationRequestedEventHandler != null)
		{
			AsciiString asciiString = new AsciiString(string.Empty);
			aURI.getSpec(asciiString.Handle);
			NavigationRequestedEventArgs navigationRequestedEventArgs = new NavigationRequestedEventArgs(asciiString.ToString());
			navigationRequestedEventHandler(this, navigationRequestedEventArgs);
			return navigationRequestedEventArgs.Cancel;
		}
		return true;
	}

	bool nsIURIContentListener.doContent(string aContentType, bool aIsContentPreferred, nsIRequest aRequest, out nsIStreamListener aContentHandler)
	{
		aContentHandler = null;
		return true;
	}

	bool nsIURIContentListener.isPreferred(string aContentType, ref string aDesiredContentType)
	{
		return true;
	}

	bool nsIURIContentListener.canHandleContent(string aContentType, bool aIsContentPreferred, ref string aDesiredContentType)
	{
		return true;
	}

	[return: MarshalAs(UnmanagedType.Interface)]
	IntPtr nsIURIContentListener.getLoadCookie()
	{
		return IntPtr.Zero;
	}

	void nsIURIContentListener.setLoadCookie([MarshalAs(UnmanagedType.Interface)] IntPtr value)
	{
	}

	nsIURIContentListener nsIURIContentListener.getParentContentListener()
	{
		return null;
	}

	void nsIURIContentListener.setParentContentListener(nsIURIContentListener value)
	{
	}

	public void AddHandler(NavigationRequestedEventHandler value)
	{
		if ((object)Events[WebBrowser.NavigationRequested] == null && owner.Navigation != null)
		{
			nsIWebBrowser nsIWebBrowser = (nsIWebBrowser)owner.navigation.navigation;
			nsIWebBrowser.setParentURIContentListener(this);
		}
		Events.AddHandler(WebBrowser.NavigationRequested, value);
	}

	public void RemoveHandler(NavigationRequestedEventHandler value)
	{
		Events.RemoveHandler(WebBrowser.NavigationRequested, value);
	}
}
