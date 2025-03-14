using System.Xml.XPath;
using System.Xml.Xsl;
using Mono.Xml.Xsl;

namespace Mono.Xml.XPath;

internal class KeyPattern : LocationPathPattern
{
	private XsltKey key;

	public override double DefaultPriority => 0.5;

	public KeyPattern(XsltKey key)
		: base((NodeTest)null)
	{
		this.key = key;
	}

	public override bool Matches(XPathNavigator node, XsltContext ctx)
	{
		return key.PatternMatches(node, ctx);
	}
}
