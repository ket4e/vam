using System.Collections.Generic;

namespace Mono.Xml;

internal class DTDCollectionBase : DictionaryBase
{
	private DTDObjectModel root;

	protected DTDObjectModel Root => root;

	public DictionaryBase InnerHashtable => this;

	protected DTDCollectionBase(DTDObjectModel root)
	{
		this.root = root;
	}

	protected void BaseAdd(string name, DTDNode value)
	{
		Add(new KeyValuePair<string, DTDNode>(name, value));
	}

	public bool Contains(string key)
	{
		using (Enumerator enumerator = GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.Key == key)
				{
					return true;
				}
			}
		}
		return false;
	}

	protected object BaseGet(string name)
	{
		using (Enumerator enumerator = GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				KeyValuePair<string, DTDNode> current = enumerator.Current;
				if (current.Key == name)
				{
					return current.Value;
				}
			}
		}
		return null;
	}
}
