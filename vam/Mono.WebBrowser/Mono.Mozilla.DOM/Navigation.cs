using Mono.WebBrowser;
using Mono.WebBrowser.DOM;

namespace Mono.Mozilla.DOM;

internal class Navigation : DOMObject, INavigation
{
	internal nsIWebNavigation navigation;

	public bool CanGoBack
	{
		get
		{
			if (navigation == null)
			{
				return false;
			}
			navigation.getCanGoBack(out var ret);
			return ret;
		}
	}

	public bool CanGoForward
	{
		get
		{
			if (navigation == null)
			{
				return false;
			}
			navigation.getCanGoForward(out var ret);
			return ret;
		}
	}

	public int HistoryCount
	{
		get
		{
			navigation.getSessionHistory(out var ret);
			ret.getCount(out var ret2);
			return ret2;
		}
	}

	internal Document Document
	{
		get
		{
			navigation.getDocument(out var ret);
			int hashCode = ret.GetHashCode();
			if (!resources.ContainsKey(hashCode))
			{
				resources.Add(hashCode, new Document(control, ret as nsIDOMHTMLDocument));
			}
			return resources[hashCode] as Document;
		}
	}

	public Navigation(WebBrowser control, nsIWebNavigation webNav)
		: base(control)
	{
		navigation = webNav;
	}

	protected override void Dispose(bool disposing)
	{
		if (!disposed && disposing)
		{
			navigation = null;
		}
		base.Dispose(disposing);
	}

	public bool Back()
	{
		if (navigation == null)
		{
			return false;
		}
		control.Reset();
		return navigation.goBack() == 0;
	}

	public bool Forward()
	{
		if (navigation == null)
		{
			return false;
		}
		control.Reset();
		return navigation.goForward() == 0;
	}

	public void Home()
	{
		control.Reset();
		Base.Home(control);
	}

	public void Reload()
	{
		Reload(ReloadOption.None);
	}

	public void Reload(ReloadOption option)
	{
		if (navigation != null)
		{
			control.Reset();
			switch (option)
			{
			case ReloadOption.None:
				navigation.reload(0u);
				break;
			case ReloadOption.Proxy:
				navigation.reload(256u);
				break;
			case ReloadOption.Full:
				navigation.reload(512u);
				break;
			}
		}
	}

	public void Stop()
	{
		if (navigation != null)
		{
			navigation.stop(3u);
		}
	}

	public void Go(int index)
	{
		if (navigation != null && index >= 0)
		{
			navigation.getSessionHistory(out var ret);
			ret.getCount(out var ret2);
			if (index <= ret2)
			{
				control.Reset();
				navigation.gotoIndex(index);
			}
		}
	}

	public void Go(int index, bool relative)
	{
		if (relative)
		{
			navigation.getSessionHistory(out var ret);
			ret.getCount(out var _);
			ret.getIndex(out var ret3);
			index = ret3 + index;
		}
		Go(index);
	}

	public void Go(string url)
	{
		if (navigation != null)
		{
			control.Reset();
			navigation.loadURI(url, 0u, null, null, null);
		}
	}

	public void Go(string url, LoadFlags flags)
	{
		if (navigation != null)
		{
			control.Reset();
			navigation.loadURI(url, (uint)flags, null, null, null);
		}
	}

	public override int GetHashCode()
	{
		return navigation.GetHashCode();
	}
}
