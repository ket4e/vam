using System.Collections;

namespace System.Xml.XPath;

internal class XPathIteratorComparer : IComparer
{
	public static XPathIteratorComparer Instance = new XPathIteratorComparer();

	private XPathIteratorComparer()
	{
	}

	public int Compare(object o1, object o2)
	{
		XPathNodeIterator xPathNodeIterator = o1 as XPathNodeIterator;
		XPathNodeIterator xPathNodeIterator2 = o2 as XPathNodeIterator;
		if (xPathNodeIterator == null)
		{
			return -1;
		}
		if (xPathNodeIterator2 == null)
		{
			return 1;
		}
		return xPathNodeIterator.Current.ComparePosition(xPathNodeIterator2.Current) switch
		{
			XmlNodeOrder.Same => 0, 
			XmlNodeOrder.After => -1, 
			_ => 1, 
		};
	}
}
