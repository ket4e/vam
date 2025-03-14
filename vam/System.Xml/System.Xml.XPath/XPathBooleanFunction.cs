namespace System.Xml.XPath;

internal abstract class XPathBooleanFunction : XPathFunction
{
	public override XPathResultType ReturnType => XPathResultType.Boolean;

	public override object StaticValue => StaticValueAsBoolean;

	public XPathBooleanFunction(FunctionArguments args)
		: base(args)
	{
	}
}
