using System;

namespace Mono.WebBrowser;

public class CreateNewWindowEventArgs : EventArgs
{
	private bool isModal;

	public bool IsModal => isModal;

	public CreateNewWindowEventArgs(bool isModal)
	{
		this.isModal = isModal;
	}
}
