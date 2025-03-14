using System.Collections;
using System.Xml;

namespace System.Security.Cryptography.Xml;

internal class XmlDsigNodeList : XmlNodeList
{
	private ArrayList _rgNodes;

	public override int Count => _rgNodes.Count;

	public XmlDsigNodeList(ArrayList rgNodes)
	{
		_rgNodes = rgNodes;
	}

	public override IEnumerator GetEnumerator()
	{
		return _rgNodes.GetEnumerator();
	}

	public override XmlNode Item(int index)
	{
		if (index < 0 || _rgNodes.Count <= index)
		{
			return null;
		}
		return (XmlNode)_rgNodes[index];
	}
}
