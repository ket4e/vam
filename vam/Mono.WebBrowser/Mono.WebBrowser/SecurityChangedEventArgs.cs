using System;

namespace Mono.WebBrowser;

public class SecurityChangedEventArgs : EventArgs
{
	private SecurityLevel state;

	public SecurityLevel State
	{
		get
		{
			return state;
		}
		set
		{
			state = value;
		}
	}

	public SecurityChangedEventArgs(SecurityLevel state)
	{
		this.state = state;
	}
}
