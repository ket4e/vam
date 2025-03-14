using System.Collections;

namespace Mono.WebBrowser.DOM;

public interface IElementCollection : IEnumerable, IList, ICollection, INodeList
{
	new IElement this[int index] { get; set; }

	new int GetHashCode();
}
