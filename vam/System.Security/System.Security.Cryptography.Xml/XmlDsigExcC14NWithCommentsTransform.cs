namespace System.Security.Cryptography.Xml;

public class XmlDsigExcC14NWithCommentsTransform : XmlDsigExcC14NTransform
{
	public XmlDsigExcC14NWithCommentsTransform()
		: base(includeComments: true)
	{
	}

	public XmlDsigExcC14NWithCommentsTransform(string inclusiveNamespacesPrefixList)
		: base(includeComments: true, inclusiveNamespacesPrefixList)
	{
	}
}
