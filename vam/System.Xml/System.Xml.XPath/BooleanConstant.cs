namespace System.Xml.XPath;

internal class BooleanConstant : Expression
{
	private bool _value;

	public override XPathResultType ReturnType => XPathResultType.Boolean;

	internal override bool Peer => true;

	public override bool HasStaticValue => true;

	public override bool StaticValueAsBoolean => _value;

	public BooleanConstant(bool value)
	{
		_value = value;
	}

	public override string ToString()
	{
		return (!_value) ? "false()" : "true()";
	}

	public override object Evaluate(BaseIterator iter)
	{
		return _value;
	}

	public override bool EvaluateBoolean(BaseIterator iter)
	{
		return _value;
	}
}
