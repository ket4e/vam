namespace System.Xml.XPath;

internal class ExprDIV : ExprNumeric
{
	protected override string Operator => " div ";

	public override double StaticValueAsNumber => (!HasStaticValue) ? 0.0 : (_left.StaticValueAsNumber / _right.StaticValueAsNumber);

	public ExprDIV(Expression left, Expression right)
		: base(left, right)
	{
	}

	public override double EvaluateNumber(BaseIterator iter)
	{
		return _left.EvaluateNumber(iter) / _right.EvaluateNumber(iter);
	}
}
