namespace System.Xml.XPath;

internal class XPathFunctionFloor : XPathNumericFunction
{
	private Expression arg0;

	public override bool HasStaticValue => arg0.HasStaticValue;

	public override double StaticValueAsNumber => (!HasStaticValue) ? 0.0 : Math.Floor(arg0.StaticValueAsNumber);

	internal override bool Peer => arg0.Peer;

	public XPathFunctionFloor(FunctionArguments args)
		: base(args)
	{
		if (args == null || args.Tail != null)
		{
			throw new XPathException("floor takes one arg");
		}
		arg0 = args.Arg;
	}

	public override object Evaluate(BaseIterator iter)
	{
		return Math.Floor(arg0.EvaluateNumber(iter));
	}

	public override string ToString()
	{
		return string.Concat(new string[3]
		{
			"floor(",
			arg0.ToString(),
			")"
		});
	}
}
