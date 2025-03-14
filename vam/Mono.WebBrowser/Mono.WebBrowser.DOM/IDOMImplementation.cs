namespace Mono.WebBrowser.DOM;

public interface IDOMImplementation
{
	bool HasFeature(string feature, string version);

	IDocumentType CreateDocumentType(string qualifiedName, string publicId, string systemId);

	IDocument CreateDocument(string namespaceURI, string qualifiedName, IDocumentType doctype);
}
