namespace System.Xml.Serialization;

public class XmlAttributeEventArgs : EventArgs
{
	private XmlAttribute attr;

	private int lineNumber;

	private int linePosition;

	private object obj;

	private string expectedAttributes;

	public XmlAttribute Attr => attr;

	public int LineNumber => lineNumber;

	public int LinePosition => linePosition;

	public object ObjectBeingDeserialized => obj;

	public string ExpectedAttributes
	{
		get
		{
			return expectedAttributes;
		}
		internal set
		{
			expectedAttributes = value;
		}
	}

	internal XmlAttributeEventArgs(XmlAttribute attr, int lineNum, int linePos, object source)
	{
		this.attr = attr;
		lineNumber = lineNum;
		linePosition = linePos;
		obj = source;
	}
}
