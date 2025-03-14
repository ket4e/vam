using Mono.WebBrowser.DOM;

namespace Mono.Mozilla.DOM;

internal class History : DOMObject, IHistory
{
	private Navigation navigation;

	public int Count => navigation.HistoryCount;

	public History(WebBrowser control, Navigation navigation)
		: base(control)
	{
		this.navigation = navigation;
	}

	public void Back(int count)
	{
		navigation.Go(count * -1, relative: true);
	}

	public void Forward(int count)
	{
		navigation.Go(count, relative: true);
	}

	public void GoToIndex(int index)
	{
		navigation.Go(index);
	}

	public void GoToUrl(string url)
	{
		int num = -1;
		navigation.navigation.getSessionHistory(out var ret);
		int count = Count;
		for (int i = 0; i < count; i++)
		{
			ret.getEntryAtIndex(i, modifyIndex: false, out var ret2);
			ret2.getURI(out var ret3);
			AsciiString asciiString = new AsciiString(string.Empty);
			ret3.getSpec(asciiString.Handle);
			if (string.Compare(asciiString.ToString(), url, ignoreCase: true) == 0)
			{
				num = i;
				break;
			}
		}
		if (num > -1)
		{
			GoToIndex(num);
		}
	}
}
