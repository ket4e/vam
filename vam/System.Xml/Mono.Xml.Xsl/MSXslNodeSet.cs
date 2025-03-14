using System.Collections;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace Mono.Xml.Xsl;

internal class MSXslNodeSet : XPathFunction
{
	private Expression arg0;

	public override XPathResultType ReturnType => XPathResultType.NodeSet;

	internal override bool Peer => arg0.Peer;

	public MSXslNodeSet(FunctionArguments args)
		: base(args)
	{
		if (args == null || args.Tail != null)
		{
			throw new XPathException("element-available takes 1 arg");
		}
		arg0 = args.Arg;
	}

	public override object Evaluate(BaseIterator iter)
	{
		XsltCompiledContext nsm = iter.NamespaceManager as XsltCompiledContext;
		XPathNavigator xPathNavigator = ((iter.Current == null) ? null : iter.Current.Clone());
		if (!(arg0.EvaluateAs(iter, XPathResultType.Navigator) is XPathNavigator value))
		{
			if (xPathNavigator != null)
			{
				return new XsltException("Cannot convert the XPath argument to a result tree fragment.", null, xPathNavigator);
			}
			return new XsltException("Cannot convert the XPath argument to a result tree fragment.", null);
		}
		ArrayList arrayList = new ArrayList();
		arrayList.Add(value);
		return new ListIterator(arrayList, nsm);
	}
}
