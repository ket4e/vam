namespace Mono.Xml.Schema;

internal class XsdIdentityPath
{
	public XsdIdentityStep[] OrderedSteps;

	public bool Descendants;

	public bool IsAttribute => OrderedSteps.Length != 0 && OrderedSteps[OrderedSteps.Length - 1].IsAttribute;
}
