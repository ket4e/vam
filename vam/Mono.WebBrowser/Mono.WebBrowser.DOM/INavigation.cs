namespace Mono.WebBrowser.DOM;

public interface INavigation
{
	bool CanGoBack { get; }

	bool CanGoForward { get; }

	int HistoryCount { get; }

	bool Back();

	bool Forward();

	void Home();

	void Reload();

	void Reload(ReloadOption option);

	void Stop();

	void Go(int index);

	void Go(int index, bool relative);

	void Go(string url);

	void Go(string url, LoadFlags flags);
}
