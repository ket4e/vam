using System;

namespace Mono.WebBrowser;

public class ContextMenuEventArgs : EventArgs
{
	private int x;

	private int y;

	public int X => x;

	public int Y => y;

	public ContextMenuEventArgs(int x, int y)
	{
		this.x = x;
		this.y = y;
	}
}
