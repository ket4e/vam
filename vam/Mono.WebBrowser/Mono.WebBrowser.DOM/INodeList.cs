using System.Collections;

namespace Mono.WebBrowser.DOM;

public interface INodeList : IEnumerable, IList, ICollection
{
	new INode this[int index] { get; set; }

	new int GetHashCode();
}
