namespace System.Xml.XPath;

internal class XPathFunctionLast : XPathFunction
{
	public override XPathResultType ReturnType => XPathResultType.Number;

	internal override bool Peer => true;

	internal override bool IsPositional => true;

	public XPathFunctionLast(FunctionArguments args)
		: base(args)
	{
		if (args != null)
		{
			throw new XPathException("last takes 0 args");
		}
	}

	public override object Evaluate(BaseIterator iter)
	{
		return (double)iter.Count;
	}

	public override string ToString()
	{
		return "last()";
	}
}
