namespace System.Xml.XPath;

internal class ExprSLASH2 : NodeSet
{
	public Expression left;

	public NodeSet right;

	private static NodeTest DescendantOrSelfStar = new NodeTypeTest(Axes.DescendantOrSelf, XPathNodeType.All);

	public override bool RequireSorting => left.RequireSorting || right.RequireSorting;

	internal override XPathNodeType EvaluatedNodeType => right.EvaluatedNodeType;

	internal override bool IsPositional => left.IsPositional || right.IsPositional;

	internal override bool Peer => false;

	internal override bool Subtree => left is NodeSet nodeSet && nodeSet.Subtree && right.Subtree;

	public ExprSLASH2(Expression left, NodeSet right)
	{
		this.left = left;
		this.right = right;
	}

	public override Expression Optimize()
	{
		left = left.Optimize();
		right = (NodeSet)right.Optimize();
		if (right is NodeTest nodeTest && nodeTest.Axis.Axis == Axes.Child)
		{
			if (nodeTest is NodeNameTest source)
			{
				return new ExprSLASH(left, new NodeNameTest(source, Axes.Descendant));
			}
			if (nodeTest is NodeTypeTest other)
			{
				return new ExprSLASH(left, new NodeTypeTest(other, Axes.Descendant));
			}
		}
		return this;
	}

	public override string ToString()
	{
		return left.ToString() + "//" + right.ToString();
	}

	public override object Evaluate(BaseIterator iter)
	{
		BaseIterator iter2 = left.EvaluateNodeSet(iter);
		if (left.Peer && !left.RequireSorting)
		{
			iter2 = new SimpleSlashIterator(iter2, DescendantOrSelfStar);
		}
		else
		{
			BaseIterator baseIterator = new SlashIterator(iter2, DescendantOrSelfStar);
			iter2 = ((!left.RequireSorting) ? baseIterator : new SortedIterator(baseIterator));
		}
		SlashIterator iter3 = new SlashIterator(iter2, right);
		return new SortedIterator(iter3);
	}
}
