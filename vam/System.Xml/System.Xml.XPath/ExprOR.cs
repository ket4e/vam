namespace System.Xml.XPath;

internal class ExprOR : ExprBoolean
{
	protected override string Operator => "or";

	public override bool StaticValueAsBoolean => HasStaticValue && (_left.StaticValueAsBoolean || _right.StaticValueAsBoolean);

	public ExprOR(Expression left, Expression right)
		: base(left, right)
	{
	}

	public override bool EvaluateBoolean(BaseIterator iter)
	{
		if (_left.EvaluateBoolean(iter))
		{
			return true;
		}
		return _right.EvaluateBoolean(iter);
	}
}
