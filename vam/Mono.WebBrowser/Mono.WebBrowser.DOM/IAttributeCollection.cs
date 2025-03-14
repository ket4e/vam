using System.Collections;

namespace Mono.WebBrowser.DOM;

public interface IAttributeCollection : IEnumerable, IList, ICollection, INodeList
{
	IAttribute this[string name] { get; }

	bool Exists(string name);

	new int GetHashCode();
}
