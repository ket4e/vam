using System;

namespace Mono.Xml;

internal class DTDNotationDeclarationCollection : DTDCollectionBase
{
	public DTDNotationDeclaration this[string name] => BaseGet(name) as DTDNotationDeclaration;

	public DTDNotationDeclarationCollection(DTDObjectModel root)
		: base(root)
	{
	}

	public void Add(string name, DTDNotationDeclaration decl)
	{
		if (Contains(name))
		{
			throw new InvalidOperationException($"Notation declaration for {name} was already added.");
		}
		decl.SetRoot(base.Root);
		BaseAdd(name, decl);
	}
}
