namespace Mono.Xml;

internal class DTDAnyAutomata : DTDAutomata
{
	public override bool Emptiable => true;

	public DTDAnyAutomata(DTDObjectModel root)
		: base(root)
	{
	}

	public override DTDAutomata TryEndElement()
	{
		return this;
	}

	public override DTDAutomata TryStartElement(string name)
	{
		return this;
	}
}
