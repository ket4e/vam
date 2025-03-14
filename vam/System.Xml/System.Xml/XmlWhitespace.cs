using System.Xml.XPath;

namespace System.Xml;

public class XmlWhitespace : XmlCharacterData
{
	public override string LocalName => "#whitespace";

	public override string Name => "#whitespace";

	public override XmlNodeType NodeType => XmlNodeType.Whitespace;

	internal override XPathNodeType XPathNodeType => XPathNodeType.Whitespace;

	public override string Value
	{
		get
		{
			return Data;
		}
		set
		{
			if (!XmlChar.IsWhitespace(value))
			{
				throw new ArgumentException("Invalid whitespace characters.");
			}
			Data = value;
		}
	}

	public override XmlNode ParentNode => base.ParentNode;

	protected internal XmlWhitespace(string strData, XmlDocument doc)
		: base(strData, doc)
	{
	}

	public override XmlNode CloneNode(bool deep)
	{
		return new XmlWhitespace(Data, OwnerDocument);
	}

	public override void WriteContentTo(XmlWriter w)
	{
	}

	public override void WriteTo(XmlWriter w)
	{
		w.WriteWhitespace(Data);
	}
}
