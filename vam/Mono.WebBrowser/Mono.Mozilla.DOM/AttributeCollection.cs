using System;
using System.Collections;
using Mono.WebBrowser.DOM;

namespace Mono.Mozilla.DOM;

internal class AttributeCollection : NodeList, IEnumerable, IList, ICollection, IAttributeCollection, INodeList
{
	protected new nsIDOMNamedNodeMap unmanagedNodes;

	public override int Count
	{
		get
		{
			if (unmanagedNodes != null && nodes == null)
			{
				Load();
			}
			return nodeCount;
		}
	}

	public new IAttribute this[int index]
	{
		get
		{
			if (index < 0 || index >= Count)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			return nodes[index] as IAttribute;
		}
		set
		{
		}
	}

	public IAttribute this[string name]
	{
		get
		{
			for (int i = 0; i < nodes.Length; i++)
			{
				if (((IAttribute)nodes[i]).Name.Equals(name))
				{
					return nodes[i] as IAttribute;
				}
			}
			return null;
		}
	}

	public AttributeCollection(WebBrowser control, nsIDOMNamedNodeMap nodeMap)
		: base(control, loaded: true)
	{
		if (control.platform != control.enginePlatform)
		{
			unmanagedNodes = nsDOMNamedNodeMap.GetProxy(control, nodeMap);
		}
		else
		{
			unmanagedNodes = nodeMap;
		}
	}

	public AttributeCollection(WebBrowser control)
		: base(control)
	{
	}

	internal override void Load()
	{
		if (unmanagedNodes != null)
		{
			Clear();
			unmanagedNodes.getLength(out var ret);
			nodeCount = (int)ret;
			nodes = new Node[ret];
			for (int i = 0; i < ret; i++)
			{
				unmanagedNodes.item((uint)i, out var ret2);
				nodes[i] = new Attribute(control, ret2 as nsIDOMAttr);
			}
		}
	}

	public bool Exists(string name)
	{
		if (unmanagedNodes == null)
		{
			return false;
		}
		Base.StringSet(storage, name);
		unmanagedNodes.getNamedItem(storage, out var ret);
		return ret != null;
	}

	public override int GetHashCode()
	{
		if (unmanagedNodes == null)
		{
			return base.GetHashCode();
		}
		return unmanagedNodes.GetHashCode();
	}
}
