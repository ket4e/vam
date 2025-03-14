using System.IO;
using Mono.WebBrowser.DOM;

namespace Mono.Mozilla.DOM;

internal class Element : Node, IElement, INode
{
	internal new nsIDOMElement node
	{
		get
		{
			return base.node as nsIDOMElement;
		}
		set
		{
			base.node = value;
		}
	}

	public virtual string InnerText
	{
		get
		{
			nsIDOMDocumentRange nsIDOMDocumentRange = ((Document)control.Document).XPComObject as nsIDOMDocumentRange;
			nsIDOMDocumentRange.createRange(out var ret);
			ret.selectNodeContents(node);
			ret.toString(storage);
			return Base.StringGet(storage);
		}
		set
		{
			Base.StringSet(storage, value);
			node.setNodeValue(storage);
		}
	}

	public virtual string OuterText
	{
		get
		{
			nsIDOMDocumentRange nsIDOMDocumentRange = ((Document)control.Document).XPComObject as nsIDOMDocumentRange;
			nsIDOMDocumentRange.createRange(out var ret);
			node.getParentNode(out var ret2);
			ret.selectNodeContents(ret2);
			ret.toString(storage);
			return Base.StringGet(storage);
		}
		set
		{
			Base.StringSet(storage, value);
			node.getParentNode(out var ret);
			ret.setNodeValue(storage);
		}
	}

	public virtual string InnerHTML
	{
		get
		{
			return string.Empty;
		}
		set
		{
		}
	}

	public virtual string OuterHTML
	{
		get
		{
			return string.Empty;
		}
		set
		{
		}
	}

	public virtual System.IO.Stream ContentStream => null;

	public IElementCollection All
	{
		get
		{
			if (!resources.Contains("All"))
			{
				HTMLElementCollection hTMLElementCollection = new HTMLElementCollection(control);
				Recurse(hTMLElementCollection, node);
				resources.Add("All", hTMLElementCollection);
			}
			return resources["All"] as IElementCollection;
		}
	}

	public IElementCollection Children
	{
		get
		{
			if (!resources.Contains("Children"))
			{
				node.getChildNodes(out var ret);
				resources.Add("Children", new HTMLElementCollection(control, ret));
			}
			return resources["Children"] as IElementCollection;
		}
	}

	public virtual int TabIndex
	{
		get
		{
			return -1;
		}
		set
		{
		}
	}

	public virtual string TagName
	{
		get
		{
			node.getTagName(storage);
			return Base.StringGet(storage);
		}
	}

	public virtual bool Disabled
	{
		get
		{
			return false;
		}
		set
		{
		}
	}

	public virtual int ClientWidth
	{
		get
		{
			if (!(node is nsIDOMNSHTMLElement nsIDOMNSHTMLElement))
			{
				return 0;
			}
			int ret = 0;
			nsIDOMNSHTMLElement.getClientWidth(out ret);
			return ret;
		}
	}

	public virtual int ClientHeight
	{
		get
		{
			if (!(node is nsIDOMNSHTMLElement nsIDOMNSHTMLElement))
			{
				return 0;
			}
			int ret = 0;
			nsIDOMNSHTMLElement.getClientHeight(out ret);
			return ret;
		}
	}

	public virtual int ScrollHeight
	{
		get
		{
			if (!(node is nsIDOMNSHTMLElement nsIDOMNSHTMLElement))
			{
				return 0;
			}
			int ret = 0;
			nsIDOMNSHTMLElement.getScrollHeight(out ret);
			return ret;
		}
	}

	public virtual int ScrollWidth
	{
		get
		{
			if (!(node is nsIDOMNSHTMLElement nsIDOMNSHTMLElement))
			{
				return 0;
			}
			int ret = 0;
			nsIDOMNSHTMLElement.getScrollWidth(out ret);
			return ret;
		}
	}

	public virtual int ScrollLeft
	{
		get
		{
			if (!(node is nsIDOMNSHTMLElement nsIDOMNSHTMLElement))
			{
				return 0;
			}
			int ret = 0;
			nsIDOMNSHTMLElement.getScrollLeft(out ret);
			return ret;
		}
		set
		{
			if (node is nsIDOMNSHTMLElement nsIDOMNSHTMLElement)
			{
				nsIDOMNSHTMLElement.setScrollLeft(value);
			}
		}
	}

	public virtual int ScrollTop
	{
		get
		{
			if (!(node is nsIDOMNSHTMLElement nsIDOMNSHTMLElement))
			{
				return 0;
			}
			int ret = 0;
			nsIDOMNSHTMLElement.getScrollTop(out ret);
			return ret;
		}
		set
		{
			if (node is nsIDOMNSHTMLElement nsIDOMNSHTMLElement)
			{
				nsIDOMNSHTMLElement.setScrollTop(value);
			}
		}
	}

	public virtual int OffsetHeight
	{
		get
		{
			if (!(node is nsIDOMNSHTMLElement nsIDOMNSHTMLElement))
			{
				return 0;
			}
			int ret = 0;
			nsIDOMNSHTMLElement.getOffsetHeight(out ret);
			return ret;
		}
	}

	public virtual int OffsetWidth
	{
		get
		{
			if (!(node is nsIDOMNSHTMLElement nsIDOMNSHTMLElement))
			{
				return 0;
			}
			int ret = 0;
			nsIDOMNSHTMLElement.getOffsetWidth(out ret);
			return ret;
		}
	}

	public virtual int OffsetLeft
	{
		get
		{
			if (!(node is nsIDOMNSHTMLElement nsIDOMNSHTMLElement))
			{
				return 0;
			}
			int ret = 0;
			nsIDOMNSHTMLElement.getOffsetLeft(out ret);
			return ret;
		}
	}

	public virtual int OffsetTop
	{
		get
		{
			if (!(node is nsIDOMNSHTMLElement nsIDOMNSHTMLElement))
			{
				return 0;
			}
			int ret = 0;
			nsIDOMNSHTMLElement.getOffsetTop(out ret);
			return ret;
		}
	}

	public virtual IElement OffsetParent
	{
		get
		{
			if (!(node is nsIDOMNSHTMLElement nsIDOMNSHTMLElement))
			{
				return null;
			}
			nsIDOMNSHTMLElement.getOffsetParent(out var ret);
			if (ret is nsIDOMHTMLElement)
			{
				return new HTMLElement(control, ret as nsIDOMHTMLElement);
			}
			return new Element(control, ret);
		}
	}

	internal int Top
	{
		get
		{
			((nsIDOMNSHTMLElement)node).getOffsetTop(out var ret);
			return ret;
		}
	}

	internal int Left
	{
		get
		{
			((nsIDOMNSHTMLElement)node).getOffsetLeft(out var ret);
			return ret;
		}
	}

	internal int Width
	{
		get
		{
			((nsIDOMNSHTMLElement)node).getOffsetWidth(out var ret);
			return ret;
		}
	}

	internal int Height
	{
		get
		{
			((nsIDOMNSHTMLElement)node).getOffsetHeight(out var ret);
			return ret;
		}
	}

	public Element(WebBrowser control, nsIDOMElement domElement)
		: base(control, domElement)
	{
		node = domElement;
	}

	protected override void Dispose(bool disposing)
	{
		if (!disposed && disposing)
		{
			node = null;
		}
		base.Dispose(disposing);
	}

	public virtual IElement AppendChild(IElement child)
	{
		Element element = (Element)child;
		node.appendChild(element.node, out var ret);
		return new Element(control, ret as nsIDOMElement);
	}

	private void Recurse(HTMLElementCollection col, nsIDOMNode parent)
	{
		parent.getChildNodes(out var ret);
		ret.getLength(out var ret2);
		for (int i = 0; i < ret2; i++)
		{
			ret.item((uint)i, out var ret3);
			ret3.getNodeType(out var ret4);
			if (ret4 == 1)
			{
				col.Add(new HTMLElement(control, (nsIDOMHTMLElement)ret3));
				Recurse(col, ret3);
			}
		}
	}

	public void Blur()
	{
		if (node is nsIDOMNSHTMLElement nsIDOMNSHTMLElement)
		{
			nsIDOMNSHTMLElement.blur();
		}
	}

	public void Focus()
	{
		if (node is nsIDOMNSHTMLElement nsIDOMNSHTMLElement)
		{
			nsIDOMNSHTMLElement.focus();
		}
	}

	public IElementCollection GetElementsByTagName(string name)
	{
		if (!resources.Contains("GetElementsByTagName" + name))
		{
			node.getElementsByTagName(storage, out var ret);
			resources.Add("GetElementsByTagName" + name, new HTMLElementCollection(control, ret));
		}
		return resources["GetElementsByTagName" + name] as IElementCollection;
	}

	public override int GetHashCode()
	{
		return hashcode;
	}

	public virtual bool HasAttribute(string name)
	{
		Base.StringSet(storage, name);
		node.hasAttribute(storage, out var ret);
		return ret;
	}

	public virtual string GetAttribute(string name)
	{
		UniString uniString = new UniString(string.Empty);
		Base.StringSet(storage, name);
		node.getAttribute(storage, uniString.Handle);
		return uniString.ToString();
	}

	public void ScrollIntoView(bool alignWithTop)
	{
		if (node is nsIDOMNSHTMLElement nsIDOMNSHTMLElement)
		{
			nsIDOMNSHTMLElement.scrollIntoView(alignWithTop);
		}
	}

	public virtual void SetAttribute(string name, string value)
	{
		UniString uniString = new UniString(value);
		Base.StringSet(storage, name);
		node.setAttribute(storage, uniString.Handle);
	}

	virtual string IElement.get_Style()
	{
		return base.Style;
	}

	virtual void IElement.set_Style(string value)
	{
		base.Style = value;
	}
}
