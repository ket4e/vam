namespace System.Xml.XPath;

internal class ExprMINUS : ExprNumeric
{
	protected override string Operator => "-";

	public override double StaticValueAsNumber => (!HasStaticValue) ? 0.0 : (_left.StaticValueAsNumber - _right.StaticValueAsNumber);

	public ExprMINUS(Expression left, Expression right)
		: base(left, right)
	{
	}

	public override double EvaluateNumber(BaseIterator iter)
	{
		return _left.EvaluateNumber(iter) - _right.EvaluateNumber(iter);
	}
}
