namespace Mono.WebBrowser.DOM;

public interface IDocumentType : INode
{
	string Name { get; }

	INamedNodeMap Entities { get; }

	INamedNodeMap Notations { get; }

	string PublicId { get; }

	string SystemId { get; }

	string InternalSubset { get; }
}
