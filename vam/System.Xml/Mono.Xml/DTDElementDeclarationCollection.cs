using System.Xml.Schema;

namespace Mono.Xml;

internal class DTDElementDeclarationCollection : DTDCollectionBase
{
	public DTDElementDeclaration this[string name] => Get(name);

	public DTDElementDeclarationCollection(DTDObjectModel root)
		: base(root)
	{
	}

	public DTDElementDeclaration Get(string name)
	{
		return BaseGet(name) as DTDElementDeclaration;
	}

	public void Add(string name, DTDElementDeclaration decl)
	{
		if (Contains(name))
		{
			base.Root.AddError(new XmlSchemaException($"Element declaration for {name} was already added.", null));
			return;
		}
		decl.SetRoot(base.Root);
		BaseAdd(name, decl);
	}
}
