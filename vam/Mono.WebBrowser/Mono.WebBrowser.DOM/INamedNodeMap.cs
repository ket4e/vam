using System.Collections;

namespace Mono.WebBrowser.DOM;

public interface INamedNodeMap : IEnumerable, IList, ICollection
{
	INode this[string name] { get; set; }

	new INode this[int index] { get; set; }

	INode this[string namespaceURI, string localName] { get; set; }

	INode RemoveNamedItem(string name);

	INode RemoveNamedItemNS(string namespaceURI, string localName);
}
