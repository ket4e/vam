using System.Xml.XPath;

namespace System.Xml;

public class XmlText : XmlCharacterData
{
	public override string LocalName => "#text";

	public override string Name => "#text";

	public override XmlNodeType NodeType => XmlNodeType.Text;

	internal override XPathNodeType XPathNodeType => XPathNodeType.Text;

	public override string Value
	{
		get
		{
			return Data;
		}
		set
		{
			Data = value;
		}
	}

	public override XmlNode ParentNode => base.ParentNode;

	protected internal XmlText(string strData, XmlDocument doc)
		: base(strData, doc)
	{
	}

	public override XmlNode CloneNode(bool deep)
	{
		return OwnerDocument.CreateTextNode(Data);
	}

	public virtual XmlText SplitText(int offset)
	{
		XmlText xmlText = OwnerDocument.CreateTextNode(Data.Substring(offset));
		DeleteData(offset, Data.Length - offset);
		ParentNode.InsertAfter(xmlText, this);
		return xmlText;
	}

	public override void WriteContentTo(XmlWriter w)
	{
	}

	public override void WriteTo(XmlWriter w)
	{
		w.WriteString(Data);
	}
}
