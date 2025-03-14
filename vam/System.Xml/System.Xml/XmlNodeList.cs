using System.Collections;
using System.Runtime.CompilerServices;

namespace System.Xml;

public abstract class XmlNodeList : IEnumerable
{
	public abstract int Count { get; }

	[IndexerName("ItemOf")]
	public virtual XmlNode this[int i] => Item(i);

	public abstract IEnumerator GetEnumerator();

	public abstract XmlNode Item(int index);
}
