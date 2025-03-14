namespace Mono.WebBrowser.DOM;

public interface IAttribute : INode
{
	string Name { get; }

	new int GetHashCode();
}
