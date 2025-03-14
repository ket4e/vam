using System;

namespace Mono.WebBrowser.DOM;

public interface IWindow
{
	IDocument Document { get; }

	IWindowCollection Frames { get; }

	string Name { get; set; }

	IWindow Parent { get; }

	string StatusText { get; }

	IWindow Top { get; }

	IHistory History { get; }

	event EventHandler Load;

	event EventHandler Unload;

	event EventHandler OnFocus;

	event EventHandler OnBlur;

	event EventHandler Error;

	event EventHandler Scroll;

	void AttachEventHandler(string eventName, EventHandler handler);

	void DetachEventHandler(string eventName, EventHandler handler);

	void Focus();

	new bool Equals(object obj);

	new int GetHashCode();

	void Open(string url);

	void ScrollTo(int x, int y);
}
