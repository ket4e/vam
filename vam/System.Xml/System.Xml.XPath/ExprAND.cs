namespace System.Xml.XPath;

internal class ExprAND : ExprBoolean
{
	protected override string Operator => "and";

	public override bool StaticValueAsBoolean => HasStaticValue && _left.StaticValueAsBoolean && _right.StaticValueAsBoolean;

	public ExprAND(Expression left, Expression right)
		: base(left, right)
	{
	}

	public override bool EvaluateBoolean(BaseIterator iter)
	{
		if (!_left.EvaluateBoolean(iter))
		{
			return false;
		}
		return _right.EvaluateBoolean(iter);
	}
}
