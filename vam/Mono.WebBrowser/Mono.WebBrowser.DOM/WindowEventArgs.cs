using System;

namespace Mono.WebBrowser.DOM;

public class WindowEventArgs : EventArgs
{
	private IWindow window;

	public IWindow Window => window;

	public WindowEventArgs(IWindow window)
	{
		this.window = window;
	}
}
