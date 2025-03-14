using System.Xml;
using System.Xml.XPath;

namespace Mono.Xml.Xsl;

internal class XsltUnparsedEntityUri : XPathFunction
{
	private Expression arg0;

	public override XPathResultType ReturnType => XPathResultType.String;

	internal override bool Peer => arg0.Peer;

	public XsltUnparsedEntityUri(FunctionArguments args)
		: base(args)
	{
		if (args == null || args.Tail != null)
		{
			throw new XPathException("unparsed-entity-uri takes 1 arg");
		}
		arg0 = args.Arg;
	}

	public override object Evaluate(BaseIterator iter)
	{
		if (!(iter.Current is IHasXmlNode hasXmlNode))
		{
			return string.Empty;
		}
		XmlNode node = hasXmlNode.GetNode();
		if (node.OwnerDocument == null)
		{
			return string.Empty;
		}
		XmlDocumentType documentType = node.OwnerDocument.DocumentType;
		if (documentType == null)
		{
			return string.Empty;
		}
		if (!(documentType.Entities.GetNamedItem(arg0.EvaluateString(iter)) is XmlEntity xmlEntity))
		{
			return string.Empty;
		}
		return (xmlEntity.SystemId == null) ? string.Empty : xmlEntity.SystemId;
	}
}
