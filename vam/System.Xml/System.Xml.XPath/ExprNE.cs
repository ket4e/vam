namespace System.Xml.XPath;

internal class ExprNE : EqualityExpr
{
	protected override string Operator => "!=";

	public ExprNE(Expression left, Expression right)
		: base(left, right, trueVal: false)
	{
	}
}
