namespace Mono.CSharp;

public class SimpleAssign : Assign
{
	public SimpleAssign(Expression target, Expression source)
		: this(target, source, target.Location)
	{
	}

	public SimpleAssign(Expression target, Expression source, Location loc)
		: base(target, source, loc)
	{
	}

	private bool CheckEqualAssign(Expression t)
	{
		if (source is Assign)
		{
			Assign assign = (Assign)source;
			if (t.Equals(assign.Target))
			{
				return true;
			}
			if (assign is SimpleAssign)
			{
				return ((SimpleAssign)assign).CheckEqualAssign(t);
			}
			return false;
		}
		return t.Equals(source);
	}

	protected override Expression DoResolve(ResolveContext ec)
	{
		Expression expression = base.DoResolve(ec);
		if (expression == null || expression != this)
		{
			return expression;
		}
		if (CheckEqualAssign(target))
		{
			ec.Report.Warning(1717, 3, loc, "Assignment made to same variable; did you mean to assign something else?");
		}
		return this;
	}

	public override void FlowAnalysis(FlowAnalysisContext fc)
	{
		base.FlowAnalysis(fc);
		if (target is VariableReference variableReference)
		{
			if (variableReference.VariableInfo != null)
			{
				fc.SetVariableAssigned(variableReference.VariableInfo);
			}
		}
		else if (target is FieldExpr fieldExpr)
		{
			fieldExpr.SetFieldAssigned(fc);
		}
		else if (target is PropertyExpr propertyExpr)
		{
			propertyExpr.SetBackingFieldAssigned(fc);
		}
	}

	public override void MarkReachable(Reachability rc)
	{
		if (source is ExpressionStatement expressionStatement)
		{
			expressionStatement.MarkReachable(rc);
		}
	}
}
