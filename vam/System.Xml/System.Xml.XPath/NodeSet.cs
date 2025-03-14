namespace System.Xml.XPath;

internal abstract class NodeSet : Expression
{
	public override XPathResultType ReturnType => XPathResultType.NodeSet;

	internal abstract bool Subtree { get; }
}
