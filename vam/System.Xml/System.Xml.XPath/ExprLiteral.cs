namespace System.Xml.XPath;

internal class ExprLiteral : Expression
{
	protected string _value;

	public string Value => _value;

	public override XPathResultType ReturnType => XPathResultType.String;

	internal override bool Peer => true;

	public override bool HasStaticValue => true;

	public override string StaticValueAsString => _value;

	public ExprLiteral(string value)
	{
		_value = value;
	}

	public override string ToString()
	{
		return "'" + _value + "'";
	}

	public override object Evaluate(BaseIterator iter)
	{
		return _value;
	}

	public override string EvaluateString(BaseIterator iter)
	{
		return _value;
	}
}
