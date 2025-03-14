namespace System.Xml.XPath;

internal class XPathFunctionFalse : XPathBooleanFunction
{
	public override bool HasStaticValue => true;

	public override bool StaticValueAsBoolean => false;

	internal override bool Peer => true;

	public XPathFunctionFalse(FunctionArguments args)
		: base(args)
	{
		if (args != null)
		{
			throw new XPathException("false takes 0 args");
		}
	}

	public override object Evaluate(BaseIterator iter)
	{
		return false;
	}

	public override string ToString()
	{
		return "false()";
	}
}
