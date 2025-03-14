namespace System.Xml.XPath;

internal class ExprGE : RelationalExpr
{
	protected override string Operator => ">=";

	public ExprGE(Expression left, Expression right)
		: base(left, right)
	{
	}

	public override bool Compare(double arg1, double arg2)
	{
		return arg1 >= arg2;
	}
}
