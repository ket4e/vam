using Mono.WebBrowser;
using Mono.WebBrowser.DOM;

namespace System.Windows.Forms;

public sealed class HtmlHistory : IDisposable
{
	private bool disposed;

	private IWebBrowser webHost;

	private IHistory history;

	public int Length => webHost.Navigation.HistoryCount;

	[System.MonoTODO("Not supported, will throw NotSupportedException")]
	public object DomHistory
	{
		get
		{
			throw new NotSupportedException("Retrieving a reference to an mshtml interface is not supported. Sorry.");
		}
	}

	internal HtmlHistory(IWebBrowser webHost, IHistory history)
	{
		this.webHost = webHost;
		this.history = history;
	}

	private void Dispose(bool disposing)
	{
		if (!disposed)
		{
			disposed = true;
		}
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	public void Back(int numberBack)
	{
		history.Back(numberBack);
	}

	public void Forward(int numberForward)
	{
		history.Forward(numberForward);
	}

	public void Go(int relativePosition)
	{
		history.GoToIndex(relativePosition);
	}

	public void Go(string urlString)
	{
		history.GoToUrl(urlString);
	}

	public void Go(Uri url)
	{
		history.GoToUrl(url.ToString());
	}
}
