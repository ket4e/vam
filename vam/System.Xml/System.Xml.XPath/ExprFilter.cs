namespace System.Xml.XPath;

internal class ExprFilter : NodeSet
{
	internal Expression expr;

	internal Expression pred;

	internal Expression LeftHandSide => expr;

	internal override XPathNodeType EvaluatedNodeType => expr.EvaluatedNodeType;

	internal override bool IsPositional
	{
		get
		{
			if (pred.ReturnType == XPathResultType.Number)
			{
				return true;
			}
			return expr.IsPositional || pred.IsPositional;
		}
	}

	internal override bool Peer => expr.Peer && pred.Peer;

	internal override bool Subtree => expr is NodeSet nodeSet && nodeSet.Subtree;

	public ExprFilter(Expression expr, Expression pred)
	{
		this.expr = expr;
		this.pred = pred;
	}

	public override Expression Optimize()
	{
		expr = expr.Optimize();
		pred = pred.Optimize();
		return this;
	}

	public override string ToString()
	{
		return "(" + expr.ToString() + ")[" + pred.ToString() + "]";
	}

	public override object Evaluate(BaseIterator iter)
	{
		BaseIterator iter2 = expr.EvaluateNodeSet(iter);
		return new PredicateIterator(iter2, pred);
	}
}
