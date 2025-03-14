namespace System.Xml.XPath;

internal class AxisSpecifier
{
	protected Axes _axis;

	public XPathNodeType NodeType => _axis switch
	{
		Axes.Namespace => XPathNodeType.Namespace, 
		Axes.Attribute => XPathNodeType.Attribute, 
		_ => XPathNodeType.Element, 
	};

	public Axes Axis => _axis;

	public AxisSpecifier(Axes axis)
	{
		_axis = axis;
	}

	public override string ToString()
	{
		return _axis switch
		{
			Axes.Ancestor => "ancestor", 
			Axes.AncestorOrSelf => "ancestor-or-self", 
			Axes.Attribute => "attribute", 
			Axes.Child => "child", 
			Axes.Descendant => "descendant", 
			Axes.DescendantOrSelf => "descendant-or-self", 
			Axes.Following => "following", 
			Axes.FollowingSibling => "following-sibling", 
			Axes.Namespace => "namespace", 
			Axes.Parent => "parent", 
			Axes.Preceding => "preceding", 
			Axes.PrecedingSibling => "preceding-sibling", 
			Axes.Self => "self", 
			_ => throw new IndexOutOfRangeException(), 
		};
	}

	public BaseIterator Evaluate(BaseIterator iter)
	{
		return _axis switch
		{
			Axes.Ancestor => new AncestorIterator(iter), 
			Axes.AncestorOrSelf => new AncestorOrSelfIterator(iter), 
			Axes.Attribute => new AttributeIterator(iter), 
			Axes.Child => new ChildIterator(iter), 
			Axes.Descendant => new DescendantIterator(iter), 
			Axes.DescendantOrSelf => new DescendantOrSelfIterator(iter), 
			Axes.Following => new FollowingIterator(iter), 
			Axes.FollowingSibling => new FollowingSiblingIterator(iter), 
			Axes.Namespace => new NamespaceIterator(iter), 
			Axes.Parent => new ParentIterator(iter), 
			Axes.Preceding => new PrecedingIterator(iter), 
			Axes.PrecedingSibling => new PrecedingSiblingIterator(iter), 
			Axes.Self => new SelfIterator(iter), 
			_ => throw new IndexOutOfRangeException(), 
		};
	}
}
