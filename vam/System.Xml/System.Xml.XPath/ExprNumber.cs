namespace System.Xml.XPath;

internal class ExprNumber : Expression
{
	protected double _value;

	public override XPathResultType ReturnType => XPathResultType.Number;

	internal override bool Peer => true;

	public override bool HasStaticValue => true;

	public override double StaticValueAsNumber => XPathFunctions.ToNumber(_value);

	internal override bool IsPositional => false;

	public ExprNumber(double value)
	{
		_value = value;
	}

	public override string ToString()
	{
		return _value.ToString();
	}

	public override object Evaluate(BaseIterator iter)
	{
		return _value;
	}

	public override double EvaluateNumber(BaseIterator iter)
	{
		return _value;
	}
}
