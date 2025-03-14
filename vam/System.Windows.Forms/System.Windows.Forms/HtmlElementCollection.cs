using System.Collections;
using System.Collections.Generic;
using Mono.WebBrowser;
using Mono.WebBrowser.DOM;

namespace System.Windows.Forms;

public sealed class HtmlElementCollection : ICollection, IEnumerable
{
	private List<HtmlElement> elements;

	private IWebBrowser webHost;

	private WebBrowser owner;

	object ICollection.SyncRoot => this;

	bool ICollection.IsSynchronized => false;

	public int Count => elements.Count;

	public HtmlElement this[string elementId]
	{
		get
		{
			foreach (HtmlElement element in elements)
			{
				if (element.Id.Equals(elementId))
				{
					return element;
				}
			}
			return null;
		}
	}

	public HtmlElement this[int index]
	{
		get
		{
			if (index > elements.Count || index < 0)
			{
				return null;
			}
			return elements[index];
		}
	}

	internal HtmlElementCollection(WebBrowser owner, IWebBrowser webHost, IElementCollection col)
	{
		elements = new List<HtmlElement>();
		foreach (IElement item in col)
		{
			elements.Add(new HtmlElement(owner, webHost, item));
		}
		this.webHost = webHost;
		this.owner = owner;
	}

	private HtmlElementCollection(WebBrowser owner, IWebBrowser webHost, List<HtmlElement> elems)
	{
		elements = elems;
		this.webHost = webHost;
		this.owner = owner;
	}

	void ICollection.CopyTo(Array dest, int index)
	{
		elements.CopyTo(dest as HtmlElement[], index);
	}

	public HtmlElementCollection GetElementsByName(string name)
	{
		List<HtmlElement> list = new List<HtmlElement>();
		foreach (HtmlElement element in elements)
		{
			if (element.HasAttribute("name") && element.GetAttribute("name").Equals(name))
			{
				list.Add(new HtmlElement(owner, webHost, element.element));
			}
		}
		return new HtmlElementCollection(owner, webHost, list);
	}

	public IEnumerator GetEnumerator()
	{
		return elements.GetEnumerator();
	}
}
