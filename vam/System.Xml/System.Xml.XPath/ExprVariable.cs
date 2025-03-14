using System.Xml.Xsl;

namespace System.Xml.XPath;

internal class ExprVariable : Expression
{
	protected XmlQualifiedName _name;

	protected bool resolvedName;

	public override XPathResultType ReturnType => XPathResultType.Any;

	internal override bool Peer => false;

	public ExprVariable(XmlQualifiedName name, IStaticXsltContext ctx)
	{
		if (ctx != null)
		{
			name = ctx.LookupQName(name.ToString());
			resolvedName = true;
		}
		_name = name;
	}

	public override string ToString()
	{
		return "$" + _name.ToString();
	}

	public override XPathResultType GetReturnType(BaseIterator iter)
	{
		return XPathResultType.Any;
	}

	public override object Evaluate(BaseIterator iter)
	{
		IXsltContextVariable xsltContextVariable = null;
		if (!(iter.NamespaceManager is XsltContext xsltContext))
		{
			throw new XPathException($"XSLT context is required to resolve variable. Current namespace manager in current node-set '{iter.GetType()}' is '{((iter.NamespaceManager == null) ? null : iter.NamespaceManager.GetType())}'");
		}
		xsltContextVariable = ((!resolvedName) ? xsltContext.ResolveVariable(new XmlQualifiedName(_name.Name, _name.Namespace)) : xsltContext.ResolveVariable(_name));
		if (xsltContextVariable == null)
		{
			throw new XPathException("variable " + _name.ToString() + " not found");
		}
		object obj = xsltContextVariable.Evaluate(xsltContext);
		if (obj is XPathNodeIterator xPathNodeIterator)
		{
			return (!(xPathNodeIterator is BaseIterator)) ? new WrapperIterator(xPathNodeIterator, iter.NamespaceManager) : xPathNodeIterator;
		}
		return obj;
	}
}
