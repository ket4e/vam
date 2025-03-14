namespace System.Xml.XPath;

internal class ExprEQ : EqualityExpr
{
	protected override string Operator => "=";

	public ExprEQ(Expression left, Expression right)
		: base(left, right, trueVal: true)
	{
	}
}
