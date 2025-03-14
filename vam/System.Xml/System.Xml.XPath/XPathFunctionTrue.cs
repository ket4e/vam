namespace System.Xml.XPath;

internal class XPathFunctionTrue : XPathBooleanFunction
{
	public override bool HasStaticValue => true;

	public override bool StaticValueAsBoolean => true;

	internal override bool Peer => true;

	public XPathFunctionTrue(FunctionArguments args)
		: base(args)
	{
		if (args != null)
		{
			throw new XPathException("true takes 0 args");
		}
	}

	public override object Evaluate(BaseIterator iter)
	{
		return true;
	}

	public override string ToString()
	{
		return "true()";
	}
}
