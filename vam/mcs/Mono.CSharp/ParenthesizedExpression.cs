namespace Mono.CSharp;

public class ParenthesizedExpression : ShimExpression
{
	public ParenthesizedExpression(Expression expr, Location loc)
		: base(expr)
	{
		base.loc = loc;
	}

	protected override Expression DoResolve(ResolveContext ec)
	{
		Expression expression = expr.Resolve(ec);
		if (expression is Constant constant && constant.IsLiteral)
		{
			return Constant.CreateConstantFromValue(expression.Type, constant.GetValue(), expr.Location);
		}
		return expression;
	}

	public override Expression DoResolveLValue(ResolveContext ec, Expression right_side)
	{
		return expr.DoResolveLValue(ec, right_side);
	}

	public override object Accept(StructuralVisitor visitor)
	{
		return visitor.Visit(this);
	}

	public override bool HasConditionalAccess()
	{
		return expr.HasConditionalAccess();
	}
}
