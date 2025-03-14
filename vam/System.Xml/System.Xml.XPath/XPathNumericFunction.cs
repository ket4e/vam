namespace System.Xml.XPath;

internal abstract class XPathNumericFunction : XPathFunction
{
	public override XPathResultType ReturnType => XPathResultType.Number;

	public override object StaticValue => StaticValueAsNumber;

	internal XPathNumericFunction(FunctionArguments args)
		: base(args)
	{
	}
}
