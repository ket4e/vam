using System.Collections;

namespace Mono.WebBrowser.DOM;

public interface IWindowCollection : IEnumerable, IList, ICollection
{
	new IWindow this[int index] { get; set; }
}
