namespace System.Xml.XPath;

internal class XPathFunctionCeil : XPathNumericFunction
{
	private Expression arg0;

	public override bool HasStaticValue => arg0.HasStaticValue;

	public override double StaticValueAsNumber => (!HasStaticValue) ? 0.0 : Math.Ceiling(arg0.StaticValueAsNumber);

	internal override bool Peer => arg0.Peer;

	public XPathFunctionCeil(FunctionArguments args)
		: base(args)
	{
		if (args == null || args.Tail != null)
		{
			throw new XPathException("ceil takes one arg");
		}
		arg0 = args.Arg;
	}

	public override object Evaluate(BaseIterator iter)
	{
		return Math.Ceiling(arg0.EvaluateNumber(iter));
	}

	public override string ToString()
	{
		return string.Concat(new string[3]
		{
			"ceil(",
			arg0.ToString(),
			")"
		});
	}
}
