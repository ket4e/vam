namespace System.Xml.XPath;

internal class ExprSLASH : NodeSet
{
	public Expression left;

	public NodeSet right;

	public override bool RequireSorting => left.RequireSorting || right.RequireSorting;

	internal override XPathNodeType EvaluatedNodeType => right.EvaluatedNodeType;

	internal override bool IsPositional => left.IsPositional || right.IsPositional;

	internal override bool Peer => left.Peer && right.Peer;

	internal override bool Subtree => left is NodeSet nodeSet && nodeSet.Subtree && right.Subtree;

	public ExprSLASH(Expression left, NodeSet right)
	{
		this.left = left;
		this.right = right;
	}

	public override Expression Optimize()
	{
		left = left.Optimize();
		right = (NodeSet)right.Optimize();
		return this;
	}

	public override string ToString()
	{
		return left.ToString() + "/" + right.ToString();
	}

	public override object Evaluate(BaseIterator iter)
	{
		BaseIterator iter2 = left.EvaluateNodeSet(iter);
		if (left.Peer && right.Subtree)
		{
			return new SimpleSlashIterator(iter2, right);
		}
		BaseIterator iter3 = new SlashIterator(iter2, right);
		return new SortedIterator(iter3);
	}
}
