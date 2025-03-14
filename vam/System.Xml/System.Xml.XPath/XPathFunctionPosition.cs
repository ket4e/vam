namespace System.Xml.XPath;

internal class XPathFunctionPosition : XPathFunction
{
	public override XPathResultType ReturnType => XPathResultType.Number;

	internal override bool Peer => true;

	internal override bool IsPositional => true;

	public XPathFunctionPosition(FunctionArguments args)
		: base(args)
	{
		if (args != null)
		{
			throw new XPathException("position takes 0 args");
		}
	}

	public override object Evaluate(BaseIterator iter)
	{
		return (double)iter.CurrentPosition;
	}

	public override string ToString()
	{
		return "position()";
	}
}
