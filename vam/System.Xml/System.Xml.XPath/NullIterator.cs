namespace System.Xml.XPath;

internal class NullIterator : SelfIterator
{
	public override int CurrentPosition => 1;

	public override XPathNavigator Current => _nav;

	public NullIterator(BaseIterator iter)
		: base(iter)
	{
	}

	public NullIterator(XPathNavigator nav)
		: this(nav, null)
	{
	}

	public NullIterator(XPathNavigator nav, IXmlNamespaceResolver nsm)
		: base(nav, nsm)
	{
	}

	private NullIterator(NullIterator other)
		: base(other, clone: true)
	{
	}

	public override XPathNodeIterator Clone()
	{
		return new NullIterator(this);
	}

	public override bool MoveNextCore()
	{
		return false;
	}
}
