namespace System.Xml.XPath;

internal class XPathFunctionStringLength : XPathFunction
{
	private Expression arg0;

	public override XPathResultType ReturnType => XPathResultType.Number;

	internal override bool Peer => arg0 == null || arg0.Peer;

	public XPathFunctionStringLength(FunctionArguments args)
		: base(args)
	{
		if (args != null)
		{
			arg0 = args.Arg;
			if (args.Tail != null)
			{
				throw new XPathException("string-length takes 1 or zero args");
			}
		}
	}

	public override object Evaluate(BaseIterator iter)
	{
		string text = ((arg0 == null) ? iter.Current.Value : arg0.EvaluateString(iter));
		return (double)text.Length;
	}

	public override string ToString()
	{
		return string.Concat(new string[3]
		{
			"string-length(",
			arg0.ToString(),
			")"
		});
	}
}
