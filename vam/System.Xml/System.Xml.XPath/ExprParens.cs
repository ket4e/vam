namespace System.Xml.XPath;

internal class ExprParens : Expression
{
	protected Expression _expr;

	public override bool HasStaticValue => _expr.HasStaticValue;

	public override object StaticValue => _expr.StaticValue;

	public override string StaticValueAsString => _expr.StaticValueAsString;

	public override double StaticValueAsNumber => _expr.StaticValueAsNumber;

	public override bool StaticValueAsBoolean => _expr.StaticValueAsBoolean;

	public override XPathResultType ReturnType => _expr.ReturnType;

	internal override XPathNodeType EvaluatedNodeType => _expr.EvaluatedNodeType;

	internal override bool IsPositional => _expr.IsPositional;

	internal override bool Peer => _expr.Peer;

	public ExprParens(Expression expr)
	{
		_expr = expr;
	}

	public override Expression Optimize()
	{
		_expr.Optimize();
		return this;
	}

	public override string ToString()
	{
		return "(" + _expr.ToString() + ")";
	}

	public override object Evaluate(BaseIterator iter)
	{
		object obj = _expr.Evaluate(iter);
		XPathNodeIterator xPathNodeIterator = obj as XPathNodeIterator;
		BaseIterator baseIterator = xPathNodeIterator as BaseIterator;
		if (baseIterator == null && xPathNodeIterator != null)
		{
			baseIterator = new WrapperIterator(xPathNodeIterator, iter.NamespaceManager);
		}
		if (baseIterator != null)
		{
			return new ParensIterator(baseIterator);
		}
		return obj;
	}
}
