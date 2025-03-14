namespace System.Xml.XPath;

internal class XPathFunctionBoolean : XPathBooleanFunction
{
	private Expression arg0;

	internal override bool Peer => arg0 == null || arg0.Peer;

	public XPathFunctionBoolean(FunctionArguments args)
		: base(args)
	{
		if (args != null)
		{
			arg0 = args.Arg;
			if (args.Tail != null)
			{
				throw new XPathException("boolean takes 1 or zero args");
			}
		}
	}

	public override object Evaluate(BaseIterator iter)
	{
		if (arg0 == null)
		{
			return XPathFunctions.ToBoolean(iter.Current.Value);
		}
		return arg0.EvaluateBoolean(iter);
	}

	public override string ToString()
	{
		return string.Concat(new string[3]
		{
			"boolean(",
			arg0.ToString(),
			")"
		});
	}
}
