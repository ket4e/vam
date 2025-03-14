using System.Collections;

namespace Mono.WebBrowser.DOM;

public interface IStylesheetList : IEnumerable
{
	int Count { get; }

	IStylesheet this[int index] { get; set; }
}
