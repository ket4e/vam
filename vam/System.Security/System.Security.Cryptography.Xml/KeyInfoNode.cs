using System.Xml;

namespace System.Security.Cryptography.Xml;

public class KeyInfoNode : KeyInfoClause
{
	private XmlElement Node;

	public XmlElement Value
	{
		get
		{
			return Node;
		}
		set
		{
			Node = value;
		}
	}

	public KeyInfoNode()
	{
	}

	public KeyInfoNode(XmlElement node)
	{
		LoadXml(node);
	}

	public override XmlElement GetXml()
	{
		return Node;
	}

	public override void LoadXml(XmlElement value)
	{
		Node = value;
	}
}
