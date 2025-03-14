namespace Mono.WebBrowser.DOM;

public interface IHistory
{
	int Count { get; }

	void Back(int count);

	void Forward(int count);

	void GoToIndex(int index);

	void GoToUrl(string url);
}
