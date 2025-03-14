namespace System.Xml.Serialization;

public class XmlNodeEventArgs : EventArgs
{
	private int linenumber;

	private int lineposition;

	private string localname;

	private string name;

	private string nsuri;

	private XmlNodeType nodetype;

	private object source;

	private string text;

	public int LineNumber => linenumber;

	public int LinePosition => lineposition;

	public string LocalName => localname;

	public string Name => name;

	public string NamespaceURI => nsuri;

	public XmlNodeType NodeType => nodetype;

	public object ObjectBeingDeserialized => source;

	public string Text => text;

	internal XmlNodeEventArgs(int linenumber, int lineposition, string localname, string name, string nsuri, XmlNodeType nodetype, object source, string text)
	{
		this.linenumber = linenumber;
		this.lineposition = lineposition;
		this.localname = localname;
		this.name = name;
		this.nsuri = nsuri;
		this.nodetype = nodetype;
		this.source = source;
		this.text = text;
	}
}
